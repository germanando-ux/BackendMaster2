using BackendMaster2.Modules.Dtos;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface IMasterDataRepository
{
    Task AddMarketplaceAsync(Marketplace newMarketplace);
    Task DeleteMarketplaceAsync(Guid id);
    Task<IEnumerable<Marketplace>> GetAllMarketplacesAsync(BaseQueryDto query);
    Task<Marketplace> GetMarketplaceByIdAsync(Guid id);
    Task<Marketplace> UpdateMarketplaceAsync(Marketplace existing);



    Task AddDataSourceAsync(DataSource newDataSource);
    Task DeleteDataSourceAsync(Guid id);
    Task<IEnumerable<DataSource>> GetAllDataSourcesAsync(BaseQueryDto query);
    Task<DataSource> GetDataSourceByIdAsync(Guid id);
    Task<DataSource> UpdateDataSourceAsync(DataSource existing);
}
