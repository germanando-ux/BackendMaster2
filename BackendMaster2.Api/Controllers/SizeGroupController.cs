using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SizeGroupController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly SizeGroupValidator _sizeGroupValidator;

    public SizeGroupController(ICatalogService catalogService, SizeGroupValidator sizeGroupValidator)
    {
        _catalogService = catalogService;
        _sizeGroupValidator = sizeGroupValidator;
    }

    [Authorize]
    [HttpGet("GetAllSizeGroups")]
    public async Task<ActionResult<IEnumerable<SizeGroupDto>>> GetAllSizeGroups([FromQuery] BaseQueryDto query)
    {
        var sizeGroups = await _catalogService.GetAllSizeGroupsAsync(query);
        return Ok(sizeGroups);
    }

    [Authorize]
    [HttpGet("GetSizeGroupById/{id}")]
    public async Task<ActionResult<SizeGroupDto?>> GetSizeGroupById(Guid id)
    {
        var sizeGroup = await _catalogService.GetSizeGroupByIdAsync(id);
        return Ok(sizeGroup);
    }

    [Authorize]
    [HttpPost("CreateSizeGroup")]
    public async Task<ActionResult<SizeGroupDto>> CreateSizeGroup(SizeGroupDto sizeGroupDto)
    {
        await _sizeGroupValidator.ValidateAndThrowAsync(sizeGroupDto);

        SizeGroupDto created = await _catalogService.CreateSizeGroupAsync(sizeGroupDto);
        return CreatedAtAction(nameof(GetSizeGroupById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateSizeGroup/{id}")]
    public async Task<ActionResult<SizeGroupDto?>> UpdateSizeGroup(Guid id, SizeGroupDto sizeGroupDto)
    {
        await _sizeGroupValidator.ValidateAndThrowAsync(sizeGroupDto);

        SizeGroupDto? updated = await _catalogService.UpdateSizeGroupAsync(sizeGroupDto);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteSizeGroup/{id}")]
    public async Task<ActionResult<bool>> DeleteSizeGroup(Guid id)
    {
        await _catalogService.DeleteSizeGroupAsync(id);
        return Ok(true);
    }
}