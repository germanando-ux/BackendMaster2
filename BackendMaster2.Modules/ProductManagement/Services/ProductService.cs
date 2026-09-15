using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.ProductManagement.Services
{
    /// <summary>
    /// Implementación de los casos de uso del módulo ProductManagement.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        // Depende del CONTRATO, no de la clase: el repositorio real lo inyecta el DI.
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null)
            {
                throw new NotFoundException($"No existe ningún producto con el id {id}.");
            }

            return product;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            // REGLA DE NEGOCIO: el SKU no puede estar duplicado.
            var existing = await _repository.GetBySkuAsync(product.Sku);
            if (existing is not null)
            {
                throw new DuplicateResourceException($"Ya existe un producto con el SKU '{product.Sku}'.");
            }

            await _repository.AddAsync(product);
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            // 1. Cargo la entidad RASTREADA (la que vive en la memoria del DbContext).
            var existing = await _repository.GetByIdAsync(product.Id);
            if (existing is null)
            {
                throw new NotFoundException($"No existe ningún producto con el id {product.Id}.");
            }

            // 2. REGLA: si el SKU cambió, el nuevo también debe estar libre.
            if (!string.Equals(existing.Sku, product.Sku, StringComparison.OrdinalIgnoreCase))
            {
                var skuOcupado = await _repository.GetBySkuAsync(product.Sku);
                if (skuOcupado is not null)
                {
                    throw new DuplicateResourceException($"Ya existe un producto con el SKU '{product.Sku}'.");
                }
            }

            // 3. Copio los cambios SOBRE la entidad rastreada: el tracker los
            //    detectará y el SaveChanges del repositorio generará el UPDATE.
            existing.Sku = product.Sku;
            existing.Name = product.Name;
            // ...resto de propiedades editables de tu Product

            await _repository.UpdateAsync(existing);
            return existing;
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
            {
                throw new NotFoundException($"No existe ningún producto con el id {id}.");
            }

            await _repository.DeleteAsync(id);
        }
    }
}
