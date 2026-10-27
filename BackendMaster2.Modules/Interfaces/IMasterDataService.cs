using BackendMaster2.Modules.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface IMasterDataService
{
    Task<MarketplaceDto> CreateMarketplaceAsync(MarketplaceDto marketplace);
    Task DeleteMarketplaceAsync(Guid id);
    Task<IEnumerable<MarketplaceDto>> GetAllMarketplacesAsync(BaseQueryDto query);
    Task<MarketplaceDto?> GetMarketplaceByIdAsync(Guid id);
    Task<MarketplaceDto?> UpdateMarketplaceAsync(MarketplaceDto marketplace);

    Task<DataSourceDto> CreateDataSourceAsync(DataSourceDto dataSource);
    Task DeleteDataSourceAsync(Guid id);
    Task<IEnumerable<DataSourceDto>> GetAllDataSourcesAsync(BaseQueryDto query);
    Task<DataSourceDto?> GetDataSourceByIdAsync(Guid id);
    Task<DataSourceDto?> UpdateDataSourceAsync(DataSourceDto dataSource);
}
