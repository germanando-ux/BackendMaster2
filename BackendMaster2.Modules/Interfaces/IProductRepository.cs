using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces
{
    // CONTRATO: qué operaciones de datos ofrece el módulo.
    // El service dependerá de ESTA interfaz, no de la clase:
    // mañana podrías cambiar la implementación (p.ej. una falsa para tests)
    // sin tocar una sola línea del service.
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(BaseQueryDto query);


        /// <summary>
        /// Busca en productos y colores por similitud semántica usando vectores.
        /// Aplica filtros duros (precio, stock) y devuelve un DTO con el score de similitud.
        /// Devuelve solo los productos que superen un umbral de similitud (0.6, 60%) y limita la cantidad de resultados según el parámetro topK.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<ProductSearchHybridResult>> GetAllProductsHybridSearchAsync(QueryHybridProductDto query);
        Task<Product?> GetByIdAsync(Guid id);
        Task<ProductDetailDto?> GetProductDetailByIdAsync(Guid id);
        Task<Product?> GetBySkuAsync(string sku);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
