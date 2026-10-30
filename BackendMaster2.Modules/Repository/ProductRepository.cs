using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Shared.Domain;
using BackendMaster2.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace BackendMaster2.Modules.Repository
{
    // ÚNICO sitio del módulo donde se escriben consultas LINQ contra el DbContext.

    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

        //obtenemos la lista de propiedades públicas de la clase Color que son de tipo simple (string, int, etc.)
        //y las guardamos en un HashSet para validación rápida, es estática y se calcula una sola vez al inicio de la aplicación
        private static readonly HashSet<string> AllowedColumns = typeof(Product)
         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
         .Where(p => TypeValidationExtensions.IsSimpleType(p.PropertyType))
         .Select(p => p.Name)
         .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // El DbContext llega INYECTADO: el repositorio no crea conexiones,
        // recibe la sesión ya configurada (connection string, provider).
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los productos de la base de datos, con la opción de ordenarlos dinámicamente según los parámetros proporcionados 
        /// en el objeto ProductQueryDto. Busca por reflexión entre los campos de la clase Product.
        /// Si no se especifica un campo de ordenamiento, se devuelve la lista sin ordenar.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync(BaseQueryDto query)
        {
            IQueryable<Product> productsQuery = _context.Products.AsQueryable();
            //Si tiene para filtrar y está entre las columnas de la tabla
            if (!string.IsNullOrWhiteSpace(query.SearchTerm) && !string.IsNullOrWhiteSpace(query.SearchBy) && AllowedColumns.Contains(query.SearchBy))
            {
                productsQuery = productsQuery.Where($"{query.SearchBy}.ToLower().Contains(@0)", query.SearchTerm.ToLower());
            }
            if (query.IsActive.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.IsActive == query.IsActive.Value);
            }
            if (!string.IsNullOrEmpty(query.SortBy) && AllowedColumns.Contains(query.SortBy))
            {
                string direction = query.SortAscending ? "ascending" : "descending";
                productsQuery = productsQuery.OrderBy($"{query.SortBy} {direction}");
            }
            else
            {
                // Si viene vacío, o intentaron inyectar una propiedad de navegación/no existente
                productsQuery = productsQuery.OrderBy(p => p.Name); // Orden por defecto
            }

            return await productsQuery.ToListAsync();
        }

        /// <summary>
        /// Busca en productos y colores por similitud semántica usando vectores.
        /// Aplica filtros duros (precio, stock) y devuelve un DTO con el score de similitud.
        /// Devuelve solo los productos que superen un umbral de similitud (0.6, 60%) y limita la cantidad de resultados según el parámetro topK.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ProductSearchHybridResult>> GetAllProductsHybridSearchAsync(QueryHybridProductDto query)
        {
            int limit = query.topK ?? 10;

            // 1. Query base con las relaciones necesarias para el DTO de detalle
            IQueryable<Product> dbQuery = _context.Products
                .Include(p => p.ProductOptions)
                    .ThenInclude(po => po.Color);


            // Buscamos los 3 colores más cercanos semánticamente al vector del usuario

            List<Guid> matchedColorIds = new List<Guid>();
            if (query.ColorVector != null)
            {
                matchedColorIds = await _context.Color
                    .OrderBy(c => c.Embedding == null ? 1.0 : c.Embedding.CosineDistance(query.ColorVector))
                    .Take(3) // Traemos los 3 mejores "matches" de color (ej: malva, morado, violeta)
                    .Select(c => c.Id)
                    .ToListAsync();

                // Si encontramos colores, filtramos los productos para que SOLO incluyan estos colores
                if (matchedColorIds.Any())
                {
                    dbQuery = dbQuery.Where(p => p.ProductOptions.Any(po => matchedColorIds.Contains(po.ColorId)));
                }
            }

            // 2. Aplicar filtros duros (solo si tienen valor)
            if (query.MaxPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Price <= query.MaxPrice.Value);
            }

            if (query.MinStock.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Stock >= query.MinStock.Value);
            }

            // 3. Ordenación por similitud semántica (Distancia Coseno)
            // Npgsql traduce .CosineDistance() automáticamente al operador <=> de pgvector
            if (query.ProductVector != null)
            {
                //dbQuery = dbQuery.OrderBy(p =>
                //    p.Embedding == null ? 1.0 : p.Embedding.CosineDistance(query.ProductVector)
                //);

                dbQuery = dbQuery
                    .Where(p => p.Embedding != null)
                    .OrderBy(p => p.Embedding.CosineDistance(query.ProductVector));
            }
            else
            {
                // Fallback: si no hay vector, ordenamos por precio ascendente
                dbQuery = dbQuery.OrderBy(p => p.Price);
            }

            // EJECUTAMOS LA QUERY, TRAEMOS LOS CANDIDATOS A MEMORIA 
            List<Product> candidatos = await dbQuery.Take(limit).ToListAsync();

            // Hacemos el filtro por similitud semántica (Entity framework se hacía la picha un lío )
            // y proyectamos a la entidad Product a ProductSearchHybridResult que es Product + Score de similitud
            var resultadosFinales = candidatos
                .Select(p => new ProductSearchHybridResult(
                    p,
                    p.Embedding.CalculateCosineSimilarity(query.ProductVector)
                ))
                .Where(r => r.SimilarityScore >= 0.50) // <-- UMBRAL: Descarta lo que tenga menos del 60%
                .OrderByDescending(r => r.SimilarityScore) // Ordenamos de mayor a menor puntuación
                .Take(limit) // Nos quedamos solo con los 'limit' mejores
                .ToList();

            return resultadosFinales;


        }


        // BÚSQUEDA POR CLAVE: FindAsync mira primero en la memoria de EF
        // (el tracker); si no está, va a BBDD. Por clave primaria es la vía rápida.
        public async Task<Product?> GetByIdAsync(Guid id)
        {
             return await _context.Products
            .Include(p => p.ProductOptions)
            .ThenInclude(po => po.Color)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .Include(p=> p.ProductMarketplaces)
            .ThenInclude(pm => pm.Marketplace)
            .Include(p => p.ProductDataSources)
            .ThenInclude(pds => pds.DataSource)
            .Include(p => p.ProductSizes)
            .ThenInclude(ps => ps.Size)
            .ThenInclude(s => s.SizeGroup)
            .FirstOrDefaultAsync(p => p.Id == id);
        }
        

        // BÚSQUEDA POR CAMPO NO CLAVE: se traduce a WHERE "Sku" = @p.
        public async Task<Product?> GetBySkuAsync(string sku)
        {

            return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku);

        }

        // INSERT: Add() marca el objeto como "pendiente de insertar";
        // SaveChanges() ejecuta el INSERT de verdad.
        public async Task CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // UPDATE: no hay ninguna línea que "meta" datos ni query UPDATE.
        // Si el objeto vino de este DbContext, el tracker ya detectó qué
        // propiedades cambiaste; SaveChanges() genera el UPDATE solo.
        public async Task UpdateAsync(Product product)
        {
            await _context.SaveChangesAsync();
        }

        // DELETE: localiza, marca como borrado, y SaveChanges ejecuta el DELETE.
        public async Task DeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

      
    }
}
