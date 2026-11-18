using AutoMapper;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Modules.Repository;
using BackendMaster2.Shared.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BackendMaster2.Modules.Services;

/// <summary>
/// Service de maestros
/// </summary>
public class MasterDataService : IMasterDataService
{
    private readonly IMasterDataRepository _MasterDataRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;


    public MasterDataService(
        IMasterDataRepository MasterDataRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _MasterDataRepository = MasterDataRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    //MARKETPLACES METHODS

    public async Task<IEnumerable<MarketplaceDto>> GetAllMarketplacesAsync(BaseQueryDto query)
    {
        string cachekey = $"maestros:marketplaces:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);

        if (isBaseQuery)
        {
            List<MarketplaceDto> cachedMarketplaces = await _cacheService.GetAsync<List<MarketplaceDto>>(cachekey);
            if (cachedMarketplaces != null)
            {
                return cachedMarketplaces;
            }
        }

        IEnumerable<Marketplace> marketplaces = await _MasterDataRepository.GetAllMarketplacesAsync(query);
        List<MarketplaceDto> marketplacesDto = marketplaces.Select(m => _mapper.Map<MarketplaceDto>(m)).ToList();

        if (isBaseQuery)
        {
            await _cacheService.SetAsync(cachekey, marketplacesDto);
        }
        return marketplacesDto;
    }

    public async Task<MarketplaceDto?> GetMarketplaceByIdAsync(Guid id)
    {
        Marketplace marketplace = await _MasterDataRepository.GetMarketplaceByIdAsync(id);
        return marketplace != null ? _mapper.Map<MarketplaceDto>(marketplace) : null;
    }

    public async Task<MarketplaceDto> CreateMarketplaceAsync(MarketplaceDto marketplaceDto)
    {
        Marketplace newMarkePlace = _mapper.Map<Marketplace>(marketplaceDto);
        await _MasterDataRepository.AddMarketplaceAsync(newMarkePlace);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:marketplaces:actives");
        await _cacheService.RemoveAsync("maestros:marketplaces:all");

        return _mapper.Map<MarketplaceDto>(newMarkePlace);
    }

    public async Task<MarketplaceDto?> UpdateMarketplaceAsync(MarketplaceDto marketplace)
    {
        Marketplace existingMarketplace = await _MasterDataRepository.GetMarketplaceByIdAsync(marketplace.Id);
        if (existingMarketplace == null)
        {
            throw new KeyNotFoundException($"No existe ningun Marketplace con ID {marketplace.Id}");
        }

        _mapper.Map(marketplace, existingMarketplace);
        await _MasterDataRepository.UpdateMarketplaceAsync(existingMarketplace);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:marketplaces:actives");
        await _cacheService.RemoveAsync("maestros:marketplaces:all");

        return _mapper.Map<MarketplaceDto>(existingMarketplace);
    }

    public async Task DeleteMarketplaceAsync(Guid id)
    {
        var existingMarketplace = await _MasterDataRepository.GetMarketplaceByIdAsync(id);
        if (existingMarketplace == null)
        {
            throw new KeyNotFoundException($"No existe ningun Marketplace con ID {id}");
        }
        await _MasterDataRepository.DeleteMarketplaceAsync(id);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:marketplaces:actives");
        await _cacheService.RemoveAsync("maestros:marketplaces:all");
    }

    //DATASOURCE METHODS
    public async Task<IEnumerable<DataSourceDto>> GetAllDataSourcesAsync(BaseQueryDto query)
    {
        string cachekey = $"maestros:datasources:{(query.IsActive == true ? "actives" : "all")}";
        bool isBaseQuery = string.IsNullOrWhiteSpace(query.SearchTerm);

        if (isBaseQuery)
        {
            List<DataSourceDto> cachedDataSources = await _cacheService.GetAsync<List<DataSourceDto>>(cachekey);
            if (cachedDataSources != null)
            {
                return cachedDataSources;
            }
        }

        IEnumerable<DataSource> dataSources = await _MasterDataRepository.GetAllDataSourcesAsync(query);
        List<DataSourceDto> dataSourcesDto = dataSources.Select(d => _mapper.Map<DataSourceDto>(d)).ToList();

        if (isBaseQuery)
        {
            await _cacheService.SetAsync(cachekey, dataSourcesDto);
        }
        return dataSourcesDto;
    }

    public async Task<DataSourceDto?> GetDataSourceByIdAsync(Guid id)
    {
        DataSource dataSource = await _MasterDataRepository.GetDataSourceByIdAsync(id);
        return dataSource != null ? _mapper.Map<DataSourceDto>(dataSource) : null;
    }

    public async Task<DataSourceDto> CreateDataSourceAsync(DataSourceDto dataSource)
    {
        DataSource newDataSource = _mapper.Map<DataSource>(dataSource);
        await _MasterDataRepository.AddDataSourceAsync(newDataSource);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:datasources:actives");
        await _cacheService.RemoveAsync("maestros:datasources:all");

        return _mapper.Map<DataSourceDto>(newDataSource);
    }

    public async Task<DataSourceDto?> UpdateDataSourceAsync(DataSourceDto dataSource)
    {
        DataSource existingDataSource = await _MasterDataRepository.GetDataSourceByIdAsync(dataSource.Id);
        if (existingDataSource == null)
        {
            throw new KeyNotFoundException($"No existe ningun DataSource con ID {dataSource.Id}");
        }

        _mapper.Map(dataSource, existingDataSource);
        await _MasterDataRepository.UpdateDataSourceAsync(existingDataSource);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:datasources:actives");
        await _cacheService.RemoveAsync("maestros:datasources:all");

        return _mapper.Map<DataSourceDto>(existingDataSource);
    }

    public async Task DeleteDataSourceAsync(Guid id)
    {
        var existingDataSource = await _MasterDataRepository.GetDataSourceByIdAsync(id);
        if (existingDataSource == null)
        {
            throw new KeyNotFoundException($"No existe ningun DataSource con ID {id}");
        }
        await _MasterDataRepository.DeleteDataSourceAsync(id);

        // Invalidación de caché
        await _cacheService.RemoveAsync("maestros:datasources:actives");
        await _cacheService.RemoveAsync("maestros:datasources:all");

    }
}
