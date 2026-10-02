using AutoMapper;
using BackendMaster2.Modules.CatalogManagement.Dtos;
using BackendMaster2.Modules.CatalogManagement.Interfaces;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Services
{
    /// <summary>
    /// Implementación de los casos de uso del módulo ProductManagement.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        // Depende del CONTRATO, no de la clase: el repositorio real lo inyecta el DI.
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => _mapper.Map<ProductDto>(p));
        }

        public async Task<ProductDetailDto> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
            {
                throw new NotFoundException($"No existe ningún producto con el id {id}.");
            }

            return _mapper.Map<ProductDetailDto>(product);
        }

        public async Task<ProductDto> CreateAsync(ProductDto productDto)
        {
            // REGLA DE NEGOCIO: el SKU no puede estar duplicado.
            var existing = await _productRepository.GetBySkuAsync(productDto.Sku);
            if (existing is not null)
            {
                throw new DuplicateResourceException($"Ya existe un producto con el SKU '{productDto.Sku}'.");
            }

            // 1. Mapeamos los campos básicos (Sku, Name, Price, etc.) a la entidad
            Product newProduct = _mapper.Map<Product>(productDto);

            // 2. Traducimos la lista de ColorIds del DTO a entidades ProductOption
            newProduct.ProductOptions = productDto.ColorIds.Select(colorId => new ProductOption
            {
                ProductId = newProduct.Id, // Se asignará al guardar, pero lo ponemos por claridad
                ColorId = colorId
            }).ToList();

            // 3. Guardamos la entidad (EF Core insertará el producto y sus opciones en cascada)
            await _productRepository.AddAsync(newProduct);
            // 4. Devolvemos el DTO con el Id ya generado por la base de datos
            return _mapper.Map<ProductDto>(newProduct);
        }

        public async Task<ProductDto> UpdateAsync(ProductDto productDto)
        {
            // 1. Cargo la entidad RASTREADA (la que vive en la memoria del DbContext).
            Product? existing = await _productRepository.GetByIdAsync(productDto.Id);
            if (existing is null)
            {
                throw new NotFoundException($"No existe ningún producto con el id {productDto.Id}.");
            }

            // 2. REGLA: si el SKU cambió, el nuevo también debe estar libre.
            if (!string.Equals(existing.Sku, productDto.Sku, StringComparison.OrdinalIgnoreCase))
            {
                var skuOcupado = await _productRepository.GetBySkuAsync(productDto.Sku);
                if (skuOcupado is not null)
                {
                    throw new DuplicateResourceException($"Ya existe un producto con el SKU '{productDto.Sku}'.");
                }
            }

            // 3. Copio los cambios SOBRE la entidad rastreada: el tracker los
            //    detectará y el SaveChanges del repositorio generará el UPDATE.

            existing.Sku = productDto.Sku;
            existing.Name = productDto.Name;
            existing.Description = productDto.Description;
            existing.Price = productDto.Price;
            existing.Stock = productDto.Stock;
            existing.IsActive = productDto.IsActive;

            // 4. LÓGICA DE COLORES (Patrón Clear + AddRange)
            // Borramos las opciones actuales y añadimos las nuevas. EF Core se encarga de los DELETE/INSERT.
            existing.ProductOptions.Clear();
            foreach (var colorId in productDto.ColorIds)
            {
                existing.ProductOptions.Add(new ProductOption { ColorId = colorId });
            }

            // 5. Guardamos
            await _productRepository.UpdateAsync(existing);

            return _mapper.Map<ProductDto>(existing);


        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing is null)
            {
                throw new NotFoundException($"No existe ningún producto con el id {id}.");
            }

            await _productRepository.DeleteAsync(id);
        }
    }
}
