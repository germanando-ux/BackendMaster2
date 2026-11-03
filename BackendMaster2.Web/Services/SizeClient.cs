using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;

public class SizeClient : ISizeClient
{
    private readonly IApiClient _apiClient;
    public SizeClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }


    public async Task<ApiResult<SizeDto>> CreateSizeAsync(SizeDto size)
    {
        return await _apiClient.SendAsync<SizeDto>(HttpMethod.Post, $"api/Size/CreateSize", size);

    }

    public async Task<ApiResult<bool>> DeleteSizeAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/Size/DeleteSize/{id}");
    }

    public async Task<ApiResult<List<SizeDto>>> GetAllSizesAsync(BaseQueryDto? query = null)
    {
        string urlParameters = _apiClient.CreateUrlParameters(query);
        return await _apiClient.SendAsync<List<SizeDto>>(HttpMethod.Get, $"api/Size/GetAllSizes{urlParameters}");
    }

    public async Task<ApiResult<SizeDto>> GetSizeByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<SizeDto>(HttpMethod.Get, $"api/Size/GetSizeById/{id}");
    }

    public async Task<ApiResult<SizeDto>> UpdateSizeAsync(Guid id, SizeDto size)
    {
        return await _apiClient.SendAsync<SizeDto>(HttpMethod.Put, $"api/Size/UpdateSize/{id}", size);
    }
}
