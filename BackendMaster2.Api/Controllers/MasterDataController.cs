using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Modules.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly IMasterDataService _masterDataService;
    private readonly MarketplaceValidator _marketplaceValidator;
    private readonly DataSourceValidator _dataSourceValidator;

    public MasterDataController(IMasterDataService masterDataService, MarketplaceValidator marketplaceValidator, DataSourceValidator dataSourceValidator)
    {
        _masterDataService = masterDataService;
        _marketplaceValidator = marketplaceValidator;
        _dataSourceValidator = dataSourceValidator;
    }

    //MARKETPLACES ENDPOINTS
    [Authorize]
    [HttpGet("GetAllMarketplaces")]
    public async Task<ActionResult<IEnumerable<MarketplaceDto>>> GetAllMarketplaces([FromQuery] BaseQueryDto query)
    {
        var marketplaces = await _masterDataService.GetAllMarketplacesAsync(query);
        return Ok(marketplaces);
    }

    [Authorize]
    [HttpGet("GetMarketplaceById/{id}")]
    public async Task<ActionResult<IEnumerable<MarketplaceDto>>> GetMarketplaceById(Guid id)
    {
        var marketplaces = await _masterDataService.GetMarketplaceByIdAsync(id);
        return Ok(marketplaces);
    }

    [Authorize]
    [HttpPost("CreateMarketplace")]
    public async Task<ActionResult<MarketplaceDto>> CreateMarketplace(MarketplaceDto marketplace)
    {
        await _marketplaceValidator.ValidateAndThrowAsync(marketplace);

        MarketplaceDto created = await _masterDataService.CreateMarketplaceAsync(marketplace);
        //devuelve el marketplace creado con un código de estado 201 Created y la ubicación del recurso creado
        return CreatedAtAction(nameof(GetMarketplaceById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateMarketplace/{id}")]
    public async Task<ActionResult<MarketplaceDto>> UpdateMarketplace(Guid id, MarketplaceDto marketplace)
    {
        await _marketplaceValidator.ValidateAndThrowAsync(marketplace);

        MarketplaceDto? updated = await _masterDataService.UpdateMarketplaceAsync(marketplace);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteMarketplace/{id}")]
    public async Task<ActionResult<bool>> DeleteMarketplace(Guid id)
    {
        await _masterDataService.DeleteMarketplaceAsync(id);
        return Ok(true);
    }


    //DATASOURCE ENDPOINTS

    [Authorize]
    [HttpGet("GetAllDataSources")]
    public async Task<ActionResult<IEnumerable<DataSourceDto>>> GetAllDataSources([FromQuery] BaseQueryDto query)
    {
        var dataSources = await _masterDataService.GetAllDataSourcesAsync(query);
        return Ok(dataSources);
    }

    [Authorize]
    [HttpGet("GetDataSourceById/{id}")]
    public async Task<ActionResult<IEnumerable<DataSourceDto>>> GetDataSourceById(Guid id)
    {
        var dataSources = await _masterDataService.GetDataSourceByIdAsync(id);
        return Ok(dataSources);
    }

    [Authorize]
    [HttpPost("CreateDataSource")]
    public async Task<ActionResult<DataSourceDto>> CreateDataSource(DataSourceDto dataSource)
    {
        await _dataSourceValidator.ValidateAndThrowAsync(dataSource);

        DataSourceDto created = await _masterDataService.CreateDataSourceAsync(dataSource);
        //devuelve el datasource creado con un código de estado 201 Created y la ubicación del recurso creado
        return CreatedAtAction(nameof(GetDataSourceById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateDataSource/{id}")]
    public async Task<ActionResult<DataSourceDto>> UpdateDataSource(Guid id, DataSourceDto dataSource)
    {
        await _dataSourceValidator.ValidateAndThrowAsync(dataSource);

        DataSourceDto? updated = await _masterDataService.UpdateDataSourceAsync(dataSource);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteDataSource/{id}")]
    public async Task<ActionResult<bool>> DeleteDataSource(Guid id)
    {
        await _masterDataService.DeleteDataSourceAsync(id);
        return Ok(true);
    }



}
