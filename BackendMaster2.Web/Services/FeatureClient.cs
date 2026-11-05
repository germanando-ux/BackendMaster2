using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;

public class FeatureClient : IFeatureClient
{

    private readonly IApiClient _apiClient;
    public FeatureClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResult<FeatureDto>> CreateFeatureAsync(FeatureDto feature)
    {
        return await _apiClient.SendAsync<FeatureDto>(HttpMethod.Post, $"api/Feature/CreateFeature", feature);
    }

    public async Task<ApiResult<bool>> DeleteFeatureAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/Feature/DeleteFeature/{id}");
    }
    

    public async Task<ApiResult<List<FeatureDto>>> GetAllFeaturesAsync(BaseQueryDto? query = null)
    {
        string urlParameters = _apiClient.CreateUrlParameters(query);
        return await _apiClient.SendAsync<List<FeatureDto>>(HttpMethod.Get, $"api/Feature/GetAllFeatures{urlParameters}");        
    }

    public async Task<ApiResult<FeatureDto>> GetFeatureByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<FeatureDto>(HttpMethod.Get, $"api/Feature/GetFeatureById/{id}");
    }

    public async Task<ApiResult<FeatureDto>> UpdateFeatureAsync(Guid id, FeatureDto feature)
    {
        return await _apiClient.SendAsync<FeatureDto>(HttpMethod.Put, $"api/Feature/UpdateFeature/{id}", feature);
    }
}
