namespace BackendMaster2.Modules.Dtos;

public class ProductDetailDto
{
    // Datos básicos (aplantados)
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }

    // Maestros completos (para pintar nombres en el detalle)
    public List<ColorDto> Colors { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();

    // Relaciones con payload (para ver stock por talla, códigos en marketplaces, etc.)
    public List<ProductSizeDto> ProductSizes { get; set; } = new();
    public List<ProductDataSourceDto> ProductDataSources { get; set; } = new();
    public List<ProductMarketplaceDto> ProductMarketplaces { get; set; } = new();
    public List<ProductFeatureDto> ProductFeatures { get; set; } = new();
}
