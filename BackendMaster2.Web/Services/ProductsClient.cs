using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;


public class ProductsClient: iProductsClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiClient _apiClient;

    public ProductsClient(IHttpClientFactory httpClientFactory, ApiClient apiClient)
    {
        _httpClientFactory = httpClientFactory;
        _apiClient = apiClient;
    }

    public async Task<ProductDto> GetAllProductAsync()
    {

        HttpClient Client = _httpClientFactory.CreateClient("Api");
        ProductDto result = await _apiClient.SendAsync<ProductDto>(HttpMethod.Get, "/api/products");
        return result;
    }
}
