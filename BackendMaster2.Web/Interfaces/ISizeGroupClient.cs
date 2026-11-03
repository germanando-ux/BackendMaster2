using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface ISizeGroupClient
{
    Task<ApiResult<List<SizeGroupDto>>> GetAllSizeGroupsAsync(BaseQueryDto? query = null);
    Task<ApiResult<SizeGroupDto>> GetSizeGroupByIdAsync(Guid id);
    Task<ApiResult<SizeGroupDto>> CreateSizeGroupAsync(SizeGroupDto sizeGroup);
    Task<ApiResult<SizeGroupDto>> UpdateSizeGroupAsync(Guid id, SizeGroupDto sizeGroup);
    Task<ApiResult<bool>> DeleteSizeGroupAsync(Guid id);
}
