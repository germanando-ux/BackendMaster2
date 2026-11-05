using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface ICategoryClient
{
    Task<ApiResult<List<CategoryDto>>> GetAllCategoriesAsync(BaseQueryDto? query = null);
    Task<ApiResult<CategoryDto>> GetCategoryByIdAsync(Guid id);
    Task<ApiResult<CategoryDto>> CreateCategoryAsync(CategoryDto category);
    Task<ApiResult<CategoryDto>> UpdateCategoryAsync(Guid id, CategoryDto category);
    Task<ApiResult<bool>> DeleteCategoryAsync(Guid id);
   
}
