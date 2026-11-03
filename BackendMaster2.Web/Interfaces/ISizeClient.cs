using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface ISizeClient
{
    Task<ApiResult<List<SizeDto>>> GetAllSizesAsync(BaseQueryDto? query = null);
    Task<ApiResult<SizeDto>> GetSizeByIdAsync(Guid id);
    Task<ApiResult<SizeDto>> CreateSizeAsync(SizeDto size);
    Task<ApiResult<SizeDto>> UpdateSizeAsync(Guid id, SizeDto size);
    Task<ApiResult<bool>> DeleteSizeAsync(Guid id);
}
