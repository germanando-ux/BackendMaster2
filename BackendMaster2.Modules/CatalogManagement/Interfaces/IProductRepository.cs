using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.ProductManagement.Interface
{
    // CONTRATO: qué operaciones de datos ofrece el módulo.
    // El service dependerá de ESTA interfaz, no de la clase:
    // mañana podrías cambiar la implementación (p.ej. una falsa para tests)
    // sin tocar una sola línea del service.
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetBySkuAsync(string sku);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
