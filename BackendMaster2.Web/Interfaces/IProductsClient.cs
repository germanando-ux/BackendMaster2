using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IProductsClient
{
    Task<ApiResult<List<ProductDto>>> GetAllProductsAsync();                                      
    Task<ApiResult<ProductDto>> GetProductByIdAsync(Guid id);
    Task<ApiResult<ProductDto>> CreateProductAsync(ProductDto product);
    Task<ApiResult<ProductDto>> UpdateProductAsync(Guid id, ProductDto product);
    Task<ApiResult<ProductDto>> DeleteProductAsync(Guid id);
}