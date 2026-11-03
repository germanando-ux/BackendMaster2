using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly FeatureValidator _featureValidator;

    public FeatureController(ICatalogService catalogService, FeatureValidator featureValidator)
    {
        _catalogService = catalogService;
        _featureValidator = featureValidator;
    }

    [Authorize]
    [HttpGet("GetAllFeatures")]
    public async Task<ActionResult<IEnumerable<FeatureDto>>> GetAllFeatures([FromQuery] BaseQueryDto query)
    {
        var features = await _catalogService.GetAllFeaturesAsync(query);
        return Ok(features);
    }

    [Authorize]
    [HttpGet("GetFeatureById/{id}")]
    public async Task<ActionResult<FeatureDto?>> GetFeatureById(Guid id)
    {
        var feature = await _catalogService.GetFeatureByIdAsync(id);
        return Ok(feature);
    }

    [Authorize]
    [HttpPost("CreateFeature")]
    public async Task<ActionResult<FeatureDto>> CreateFeature(FeatureDto featureDto)
    {
        await _featureValidator.ValidateAndThrowAsync(featureDto);

        FeatureDto created = await _catalogService.CreateFeatureAsync(featureDto);
        return CreatedAtAction(nameof(GetFeatureById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateFeature/{id}")]
    public async Task<ActionResult<FeatureDto?>> UpdateFeature(Guid id, FeatureDto featureDto)
    {
        await _featureValidator.ValidateAndThrowAsync(featureDto);

        FeatureDto? updated = await _catalogService.UpdateFeatureAsync(featureDto);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteFeature/{id}")]
    public async Task<ActionResult<bool>> DeleteFeature(Guid id)
    {
        await _catalogService.DeleteFeatureAsync(id);
        return Ok(true);
    }
}