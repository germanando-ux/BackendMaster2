using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.CatalogManagement.Interfaces
{
    /// <summary>
    /// Casos de uso del módulo: devuelven datos reales.
    /// Los fallos de negocio viajan como excepciones de dominio.
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDetailDto> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(ProductDto product);
        Task<ProductDto> UpdateAsync(ProductDto product);
        Task DeleteAsync(Guid id);
    }
}
