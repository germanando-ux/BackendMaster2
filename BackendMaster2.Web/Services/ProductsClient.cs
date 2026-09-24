using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;


public class ProductsClient: IProductsClient
{
    private readonly IApiClient _apiClient;

    public ProductsClient(IApiClient apiClient)
    {        
        _apiClient = apiClient;
    }

    public async Task<ApiResult<List<ProductDto>>> GetAllProductsAsync()
    {        
       return await _apiClient.SendAsync<List<ProductDto>>(HttpMethod.Get, "api/products");
    }

    public async Task<ApiResult<ProductDto>> GetProductByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<ProductDto>(HttpMethod.Get, $"api/products/{id}");
    }

    public async Task<ApiResult<ProductDto>> CreateProductAsync(ProductDto product)
    {
        return await _apiClient.SendAsync<ProductDto>(HttpMethod.Post, $"api/products",product);
    }

    public async Task<ApiResult<ProductDto>> UpdateProductAsync(Guid id, ProductDto product)
    {
        return await _apiClient.SendAsync<ProductDto>(HttpMethod.Put, $"api/products/{id}", product);
    }


    public async Task<ApiResult<ProductDto>> DeleteProductAsync(Guid id)
    {
        return await _apiClient.SendAsync<ProductDto>(HttpMethod.Delete, $"api/products/{id}");
    }
}

