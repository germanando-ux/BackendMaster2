using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IFeatureClient
{
    Task<ApiResult<List<FeatureDto>>> GetAllFeaturesAsync(BaseQueryDto? query = null);
    Task<ApiResult<FeatureDto>> GetFeatureByIdAsync(Guid id);
    Task<ApiResult<FeatureDto>> CreateFeatureAsync(FeatureDto feature);
    Task<ApiResult<FeatureDto>> UpdateFeatureAsync(Guid id, FeatureDto feature);
    Task<ApiResult<bool>> DeleteFeatureAsync(Guid id);
}
