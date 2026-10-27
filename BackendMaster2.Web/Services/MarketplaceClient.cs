using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;
using System.Drawing;

namespace BackendMaster2.Web.Services;

public class MarketplaceClient : IMarketplaceClient
{


    private readonly IApiClient _apiClient;
    public MarketplaceClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResult<MarketplaceDto>> CreateMarketplaceAsync(MarketplaceDto marketplace)
    {
        return await _apiClient.SendAsync<MarketplaceDto>(HttpMethod.Post, $"api/MasterData/CreateMarketplace", marketplace);
    }
    
    public async Task<ApiResult<bool>> DeleteMarketplaceAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/MasterData/DeleteMarketplace/{id}");
    }

    public async Task<ApiResult<List<MarketplaceDto>>> GetAllMarketplaceAsync(BaseQueryDto? query = null)
    {
        string urlParameters = _apiClient.CreateUrlParameters(query);
        return await _apiClient.SendAsync<List<MarketplaceDto>>(HttpMethod.Get, $"api/MasterData/GetAllMarketplaces{urlParameters}");        
    }

    public async Task<ApiResult<MarketplaceDto>> GetMarketplaceByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<MarketplaceDto>(HttpMethod.Get, $"api/MasterData/GetMarketplaceById/{id}");
    }

    public async Task<ApiResult<MarketplaceDto>> UpdateMarketplaceAsync(Guid id, MarketplaceDto marketplace)
    {
        return await _apiClient.SendAsync<MarketplaceDto>(HttpMethod.Put, $"api/MasterData/UpdateMarketplace/{id}", marketplace);
    }
}
