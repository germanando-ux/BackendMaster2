using Microsoft.EntityFrameworkCore;
using BackendMaster2.Modules.Data;
using BackendMaster2.Shared.Domain;
using BackendMaster2.Modules.Interfaces;
using System.Reflection;
using System.Linq.Dynamic.Core;
using BackendMaster2.Shared.Extensions;
using BackendMaster2.Modules.Dtos;

namespace BackendMaster2.Modules.Repository
{
    // ÚNICO sitio del módulo donde se escriben consultas LINQ contra el DbContext.

    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

        //obtenemos la lista de propiedades públicas de la clase Color que son de tipo simple (string, int, etc.)
        //y las guardamos en un HashSet para validación rápida, es estática y se calcula una sola vez al inicio de la aplicación
        private static readonly HashSet<string> AllowedSortColumns = typeof(Product)
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
        public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryDto query)
        {
            IQueryable<Product> productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(query.SortBy) && AllowedSortColumns.Contains(query.SortBy))
            {
                // Si pasa el filtro, es 100% seguro usar Dynamic.Core
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

    

        // BÚSQUEDA POR CLAVE: FindAsync mira primero en la memoria de EF
        // (el tracker); si no está, va a BBDD. Por clave primaria es la vía rápida.
        public async Task<Product?> GetByIdAsync(Guid id)
        {
             return await _context.Products
            .Include(p => p.ProductOptions)
            .ThenInclude(po => po.Color)
            .FirstOrDefaultAsync(p => p.Id == id);
        }
        

        // BÚSQUEDA POR CAMPO NO CLAVE: se traduce a WHERE "Sku" = @p.
        public async Task<Product?> GetBySkuAsync(string sku)
        {

            return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku);

        }

        // INSERT: Add() marca el objeto como "pendiente de insertar";
        // SaveChanges() ejecuta el INSERT de verdad.
        public async Task AddAsync(Product product)
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
