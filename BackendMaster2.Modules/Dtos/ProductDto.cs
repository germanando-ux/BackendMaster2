namespace BackendMaster2.Modules.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;

    // Relaciones simples (sin payload): solo IDs
    public List<Guid> ColorIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();

    // Relaciones con payload: DTOs completos
    public List<ProductSizeDto> ProductSizes { get; set; } = new();
    public List<ProductDataSourceDto> ProductDataSources { get; set; } = new();
    public List<ProductMarketplaceDto> ProductMarketplaces { get; set; } = new();
    public List<ProductFeatureDto> ProductFeatures { get; set; } = new();

}
