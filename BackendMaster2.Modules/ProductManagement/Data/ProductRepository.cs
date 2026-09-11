using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace BackendMaster2.Modules.ProductManagement.Data
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

        // LECTURA: ToListAsync() ejecuta el SELECT y materializa la lista.
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // BÚSQUEDA POR CLAVE: FindAsync mira primero en la memoria de EF
        // (el tracker); si no está, va a BBDD. Por clave primaria es la vía rápida.
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
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
