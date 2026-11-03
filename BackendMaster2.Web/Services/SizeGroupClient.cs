using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;

public class SizeGroupClient : ISizeGroupClient
{

    private readonly IApiClient _apiClient;
    public SizeGroupClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResult<SizeGroupDto>> CreateSizeGroupAsync(SizeGroupDto sizeGroup)
    {
        return await _apiClient.SendAsync<SizeGroupDto>(HttpMethod.Post, $"api/SizeGroup/CreateSizeGroup", sizeGroup);
    }

    public async Task<ApiResult<bool>> DeleteSizeGroupAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/SizeGroup/DeleteSizeGroup/{id}");

    }

    public async Task<ApiResult<List<SizeGroupDto>>> GetAllSizeGroupsAsync(BaseQueryDto? query = null)
    {
        string urlParameters = _apiClient.CreateUrlParameters(query);
        return await _apiClient.SendAsync<List<SizeGroupDto>>(HttpMethod.Get, $"api/SizeGroup/GetAllSizeGroups{urlParameters}");
    }

    public async Task<ApiResult<SizeGroupDto>> GetSizeGroupByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<SizeGroupDto>(HttpMethod.Get, $"api/SizeGroup/GetSizeGroupById/{id}");

    }

    public async Task<ApiResult<SizeGroupDto>> UpdateSizeGroupAsync(Guid id, SizeGroupDto sizeGroup)
    {
        return await _apiClient.SendAsync<SizeGroupDto>(HttpMethod.Put, $"api/SizeGroup/UpdateSizeGroup/{id}", sizeGroup);        
    }
}
