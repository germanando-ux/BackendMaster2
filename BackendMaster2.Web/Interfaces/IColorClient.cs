using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IColorClient
{
    Task<ApiResult<List<ColorDto>>> GetAllColorsAsync();
    Task<ApiResult<ColorDto>> GetColorByIdAsync(Guid id);
    Task<ApiResult<ColorDto>> CreateColorAsync(ColorDto color);
    Task<ApiResult<ColorDto>> UpdateColorAsync(Guid id, ColorDto color);
    Task<ApiResult<bool>> DeleteColorAsync(Guid id);
}
