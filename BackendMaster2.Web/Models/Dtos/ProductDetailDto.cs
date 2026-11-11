namespace BackendMaster2.Web.Models.Dtos;

public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }

    public IEnumerable<ColorDto> Colors { get; set; } = null!;
    public IEnumerable<CategoryDto> Categories { get; set; } = null!;

    // Relaciones con payload (para ver stock por talla, códigos en marketplaces, etc.)
    public List<ProductSizeDto> ProductSizes { get; set; } = new();
    public List<ProductDataSourceDto> ProductDataSources { get; set; } = new();
    public List<ProductMarketplaceDto> ProductMarketplaces { get; set; } = new();
    public List<ProductFeatureDto> ProductFeatures { get; set; } = new();
}
