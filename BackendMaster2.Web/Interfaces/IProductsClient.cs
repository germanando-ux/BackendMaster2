using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IProductsClient
{
    Task<ApiResult<List<ProductDto>>> GetAllProductAsync();
    Task<ApiResult<ProductDto>> GetProductByIdAsync(int id);
    Task<ApiResult<ProductDto>> CreateProduct(ProductDto product);
    Task<ApiResult<ProductDto>> UpdateProduct(Guid id, ProductDto product);
    Task<ApiResult<ProductDto>> DeleteProduct(Guid id);
}