using BackendMaster2.Api.Validators;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Shared.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly CreateProductValidator _createValidator;
        private readonly UpdateProductValidator _updateValidator;

        // El controller solo conoce el CONTRATO del service.
        public ProductsController(IProductService service, CreateProductValidator createValidator, UpdateProductValidator updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            await _createValidator.ValidateAndThrowAsync(product);
            
            var created = await _service.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Product>> Update(Guid id, Product product)
        {
         
            await _updateValidator.ValidateAndThrowAsync(product);  
            var updated = await _service.UpdateAsync(product);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
