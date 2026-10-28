using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly CategoryValidator _CategoryValidator;

    public CategoryController(ICatalogService catalogService, CategoryValidator categoryValidator)
    {
        _catalogService = catalogService;
        _CategoryValidator = categoryValidator;
    }

    [Authorize]
    [HttpGet("GetAllCategories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories([FromQuery] BaseQueryDto query)
    {
        var categories = await _catalogService.GetAllCategoriesAsync(query);
        return Ok(categories);
    }

    [Authorize]
    [HttpGet("GetCategoryById/{id}")]
    public async Task<ActionResult<CategoryDto?>> GetCategoryById(Guid id)
    {
        var category = await _catalogService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    [Authorize]
    [HttpPost("CreateCategory")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto category)
    {
        await _CategoryValidator.ValidateAndThrowAsync(category);

        CategoryDto created = await _catalogService.CreateCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("UpdateCategory/{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, CategoryDto category)
    {
        await _CategoryValidator.ValidateAndThrowAsync(category);

        CategoryDto updated = await _catalogService.UpdateCategoryAsync(category);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("DeleteCategory/{id}")]
    public async Task<ActionResult<bool>> DeleteCategory(Guid id)
    {
        await _catalogService.DeleteCategoryAsync(id);
        return Ok(true);
    }
}