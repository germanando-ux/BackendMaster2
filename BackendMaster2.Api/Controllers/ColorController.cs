using BackendMaster2.Modules.CatalogManagement.Interfaces;
using BackendMaster2.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColorController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    public ColorController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [Authorize]
    [HttpGet("GetAllColors")]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllColors()
    {
        var colors = await _catalogService.GetAllColorsAsync();
        return Ok(colors);
    }


    [Authorize]
    [HttpGet("GetColorById/{id}")]
    public async Task<ActionResult<Product>> GetColorById(Guid id)
    {
        var color = await _catalogService.GetColorByIdAsync(id);
        return Ok(color);
    }
}
