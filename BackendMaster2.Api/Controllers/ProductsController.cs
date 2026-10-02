using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Shared.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly CreateProductValidator _createValidator;
    private readonly UpdateProductValidator _updateValidator;

    // El controller solo conoce el CONTRATO del service.
    public ProductsController(IProductService productService, CreateProductValidator createValidator, UpdateProductValidator updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductDto product)
    {
        await _createValidator.ValidateAndThrowAsync(product);

        var created = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, ProductDto product)
    {

        await _updateValidator.ValidateAndThrowAsync(product);
        var updated = await _productService.UpdateAsync(product);
        return Ok(updated);
    }
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    //[Authorize]
    //[HttpGet("GetProductDetails")]
    //public async Task<ActionResult<ProductDetailDto>> GetProductDetails(Guid id)
    //{
    //    var productDetails = await _productService.GetProductDetailsAsync(id);
    //    return Ok(productDetails);
    //}

    /// <summary>ENDPOINT TEMPORAL DE PRUEBA: borrar después de probar el catch (JsonException).</summary>
    [AllowAnonymous]
    [HttpGet("error-plano")]
    public ActionResult ErrorPlano()
    {
        Response.StatusCode = 500;
        return Content("esto no es JSON", "text/plain");
    }
}
   
