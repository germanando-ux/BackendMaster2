using BackendMaster2.Web.Models.Dtos;

namespace BackendMaster2.Web.Interfaces;

public interface IMarketplaceClient
{
    Task<ApiResult<List<MarketplaceDto>>> GetAllMarketplaceAsync(BaseQueryDto? query = null);
    Task<ApiResult<MarketplaceDto>> GetMarketplaceByIdAsync(Guid id);
    Task<ApiResult<MarketplaceDto>> CreateMarketplaceAsync(MarketplaceDto marketplace);
    Task<ApiResult<MarketplaceDto>> UpdateMarketplaceAsync(Guid id, MarketplaceDto marketplace);
    Task<ApiResult<bool>> DeleteMarketplaceAsync(Guid id);
}
