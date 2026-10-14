using Microsoft.EntityFrameworkCore;
using BackendMaster2.Modules.Data;
using BackendMaster2.Shared.Domain;
using BackendMaster2.Modules.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

namespace BackendMaster2.Modules.Repository
{
    // ÚNICO sitio del módulo donde se escriben consultas LINQ contra el DbContext.

    public class ProductRepository: IProductRepository
    {
        private readonly AppDbContext _context;

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
        public async Task<IEnumerable<Product>> GetAllProductsAsync(Dtos.ProductQueryDto query)
        {
            IQueryable<Product> productsQuery = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                //Busca en la clase property el campo que coincida con el nombre proporcionado en SortBy, ignorando mayúsculas y minúsculas
                PropertyInfo? propertyInfo = typeof(Product).GetProperty(query.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null && IsSimpleType(propertyInfo.PropertyType))
                {

                    //creamos una expresión lambda para acceder a la propiedad de ordenamiento
                    ParameterExpression parameter = Expression.Parameter(typeof(Product), "p");
                    MemberExpression propertyAccess = Expression.Property(parameter, propertyInfo);
                    LambdaExpression lambda = Expression.Lambda(propertyAccess, parameter);
                    //aplicamos el ordenamiento dinámico usando OrderBy o OrderByDescending según corresponda
                    string methodName = query.SortAscending ? "OrderBy" : "OrderByDescending";
                    MethodInfo method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(Product), propertyInfo.PropertyType);

                    productsQuery = (IQueryable<Product>)method.Invoke(null, new object[] { productsQuery, lambda });
                }
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

        /// <summary>
        /// Verifica si un tipo es "simple" (string, números, fechas, bool, Guid).
        /// Esto evita que se ordene por propiedades de navegación complejas.
        /// </summary>
        private bool IsSimpleType(Type propertyType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            return underlyingType.IsPrimitive
                || underlyingType.IsEnum
                || underlyingType == typeof(string)
                || underlyingType == typeof(decimal)
                || underlyingType == typeof(DateTime)
                || underlyingType == typeof(DateTimeOffset)
                || underlyingType == typeof(TimeSpan)
                || underlyingType == typeof(Guid);
        }
    }
}
