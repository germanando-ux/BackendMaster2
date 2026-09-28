using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Modules.CatalogManagement.Interfaces;
using BackendMaster2.Modules.CatalogManagement.Services;
using BackendMaster2.Shared.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColorController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly CreateColorValidator _createColorValidator;
    public ColorController(ICatalogService catalogService, CreateColorValidator createCatalogValidator)
    {
        _catalogService = catalogService;
        _createColorValidator = createCatalogValidator;
    }

    [Authorize]
    [HttpGet("GetAllColors")]
    public async Task<ActionResult<IEnumerable<ColorDto>>> GetAllColors()
    {
        var colors = await _catalogService.GetAllColorsAsync();
        return Ok(colors);
    }


    [Authorize]
    [HttpGet("GetColorById/{id}")]
    public async Task<ActionResult<ColorDto?>> GetColorById(Guid id)
    {
        var color = await _catalogService.GetColorByIdAsync(id);
        return Ok(color);
    }

    [Authorize]
    [HttpPost("CreateColor")]
    public async Task<ActionResult<ColorDto>> Create(ColorDto color)
    {
        await _createColorValidator.ValidateAndThrowAsync(color);

        ColorDto created = await _catalogService.CreateColorAsync(color);
        return CreatedAtAction(nameof(GetColorById), new { id = created.Id }, created);
    }


    [Authorize]
    [HttpPut("UpdateColor/{id}")]
    public async Task<ActionResult<ColorDto>> Update(Guid id, ColorDto color)
    {
        await _createColorValidator.ValidateAndThrowAsync(color);

        ColorDto? updated = await _catalogService.UpdateColorAsync(color);
        return Ok(updated);
    }

}
