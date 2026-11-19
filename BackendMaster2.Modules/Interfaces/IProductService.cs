using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces
{
    /// <summary>
    /// Casos de uso del módulo: devuelven datos reales.
    /// Los fallos de negocio viajan como excepciones de dominio.
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(BaseQueryDto query);
        Task<ProductDetailDto> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(ProductDto product);
        Task<ProductDto> UpdateAsync(ProductDto product);
        Task DeleteAsync(Guid id);

        /// <summary>
        /// LLama al servicio de chat de IA para traducir la consulta del usuario a un JSON con la intención de búsqueda de producto.
        /// Convierte en vectores la consulta de texto y la de color
        /// Llama al repositorio para hacer la búsqueda híbrida.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task ProcessEnrichDescriptionAsync(EnrichDescriptionRequestDto dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductSearchHybridDto>> GetAllProductsHybridSearchAsync(string query, CancellationToken cancellationToken = default);
    }
}
