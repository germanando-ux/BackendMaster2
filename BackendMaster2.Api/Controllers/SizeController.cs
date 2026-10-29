using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SizeController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly SizeValidator _sizeValidator;

    public SizeController(ICatalogService catalogService, SizeValidator sizeValidator)
    {
        _catalogService = catalogService;
        _sizeValidator = sizeValidator;
    }

    [Authorize]
    [HttpGet("GetAllSizes")]
    public async Task<ActionResult<IEnumerable<SizeDto>>> GetAllSizes([FromQuery] BaseQueryDto query)
    {
        var sizes = await _catalogService.GetAllSizesAsync(query);
        return Ok(sizes);
    }

    [Authorize]
    [HttpGet("GetSizeById/{id}")]
    public async Task<ActionResult<SizeDto?>> GetSizeById(Guid id)
    {
        var size = await _catalogService.GetSizeByIdAsync(id);
        return Ok(size);
    }

    [Authorize]
    [HttpPost("CreateSize")]
    public async Task<ActionResult<SizeDto>> CreateSize(SizeDto sizeDto)
    {
        await _sizeValidator.ValidateAndThrowAsync(sizeDto);

        SizeDto created = await _catalogService.CreateSizeAsync(sizeDto);
        return CreatedAtAction(nameof(GetSizeById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateSize/{id}")]
    public async Task<ActionResult<SizeDto?>> UpdateSize(Guid id, SizeDto sizeDto)
    {
        await _sizeValidator.ValidateAndThrowAsync(sizeDto);

        SizeDto? updated = await _catalogService.UpdateSizeAsync(sizeDto);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteSize/{id}")]
    public async Task<ActionResult<bool>> DeleteSize(Guid id)
    {
        await _catalogService.DeleteSizeAsync(id);
        return Ok(true);
    }
}