using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Services;

public class CategoryClient : ICategoryClient
{
    private readonly IApiClient _apiClient;
    public CategoryClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }


    public async Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryDto category)
    {
        return await _apiClient.SendAsync<CategoryDto>(HttpMethod.Post, $"api/Category/CreateCategory", category);

    }

    public async Task<ApiResult<bool>> DeleteCategoryAsync(Guid id)
    {
        return await _apiClient.SendAsync<bool>(HttpMethod.Delete, $"api/Category/DeleteCategory/{id}");
    }

    public async Task<ApiResult<List<CategoryDto>>> GetAllCategoriesAsync(BaseQueryDto? query = null)
    {
        string urlParameters = _apiClient.CreateUrlParameters(query);
        return await _apiClient.SendAsync<List<CategoryDto>>(HttpMethod.Get, $"api/Category/GetAllCategories{urlParameters}");
    }

    public async Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(Guid id)
    {
        return await _apiClient.SendAsync<CategoryDto>(HttpMethod.Get, $"api/Category/GetCategoryById/{id}");
    }

    public async Task<ApiResult<CategoryDto>> UpdateCategoryAsync(Guid id, CategoryDto category)
    {
        return await _apiClient.SendAsync<CategoryDto>(HttpMethod.Put, $"api/Category/UpdateCategory/{id}", category);
    }
}
