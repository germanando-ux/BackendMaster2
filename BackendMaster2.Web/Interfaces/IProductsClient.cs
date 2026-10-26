using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IProductsClient
{
    Task<ApiResult<List<ProductDto>>> GetAllProductsAsync(BaseQueryDto? query = null);                                      
    Task<ApiResult<ProductDetailDto>> GetProductDetailByIdAsync(Guid id);
    Task<ApiResult<ProductDto>> CreateProductAsync(ProductDto product);
    Task<ApiResult<ProductDto>> UpdateProductAsync(Guid id, ProductDto product);
    Task<ApiResult<string>> EnrichDescriptionAsync(EnrichDescriptionRequestDto request);
    Task<ApiResult<bool>> DeleteProductAsync(Guid id);
    Task<ApiResult<List<ProductSearchHybridDto>>> GetAllProductsHybridSearch(string query);
}