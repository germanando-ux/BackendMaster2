using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;

public class ColorClient : IColorClient
{
    private readonly IApiClient _apiClient;
    public ColorClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }   

    public async Task<ApiResult<ColorDto>> CreateColorAsync(ColorDto color)
    {
        return await _apiClient.SendAsync<ColorDto>(HttpMethod.Post, $"api/color/CreateColor", color);
    }

    public async Task<ApiResult<bool>> DeleteColorAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/color/DeleteColor/{id}");
    }

    public async Task<ApiResult<List<ColorDto>>> GetAllColorsAsync()
    {
        return await _apiClient.SendAsync<List<ColorDto>>(HttpMethod.Get, "api/color/GetAllColors");
    }

    public async Task<ApiResult<ColorDto>> GetColorByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<ColorDto>(HttpMethod.Get, $"api/color/GetColorById/{id}");
    }

    public async Task<ApiResult<ColorDto>> UpdateColorAsync(Guid id, ColorDto color)
    {
        return await _apiClient.SendAsync<ColorDto>(HttpMethod.Put, $"api/color/UpdateColor/{id}", color);
    }
}
