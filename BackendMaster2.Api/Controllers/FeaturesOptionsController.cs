using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureOptionController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly FeatureOptionValidator _featureOptionValidator;

    public FeatureOptionController(ICatalogService catalogService, FeatureOptionValidator featureOptionValidator)
    {
        _catalogService = catalogService;
        _featureOptionValidator = featureOptionValidator;
    }

    [Authorize]
    [HttpGet("GetAllFeatureOptions")]
    public async Task<ActionResult<IEnumerable<FeatureOptionDto>>> GetAllFeatureOptions([FromQuery] BaseQueryDto query)
    {
        var options = await _catalogService.GetAllFeatureOptionsAsync(query);
        return Ok(options);
    }

    [Authorize]
    [HttpGet("GetFeatureOptionById/{id}")]
    public async Task<ActionResult<FeatureOptionDto?>> GetFeatureOptionById(Guid id)
    {
        var option = await _catalogService.GetFeatureOptionByIdAsync(id);
        return Ok(option);
    }

    [Authorize]
    [HttpPost("CreateFeatureOption")]
    public async Task<ActionResult<FeatureOptionDto>> CreateFeatureOption(FeatureOptionDto featureOptionDto)
    {
        await _featureOptionValidator.ValidateAndThrowAsync(featureOptionDto);

        FeatureOptionDto created = await _catalogService.CreateFeatureOptionAsync(featureOptionDto);
        return CreatedAtAction(nameof(GetFeatureOptionById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateFeatureOption/{id}")]
    public async Task<ActionResult<FeatureOptionDto?>> UpdateFeatureOption(Guid id, FeatureOptionDto featureOptionDto)
    {
        await _featureOptionValidator.ValidateAndThrowAsync(featureOptionDto);

        FeatureOptionDto? updated = await _catalogService.UpdateFeatureOptionAsync(featureOptionDto);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteFeatureOption/{id}")]
    public async Task<ActionResult<bool>> DeleteFeatureOption(Guid id)
    {
        await _catalogService.DeleteFeatureOptionAsync(id);
        return Ok(true);
    }
}