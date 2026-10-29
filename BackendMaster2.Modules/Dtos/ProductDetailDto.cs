namespace BackendMaster2.Modules.Dtos;

public class ProductDetailDto
{    
    public ProductDto Product { get; set; } = null!;
    public IEnumerable<ColorDto> Colors { get; set; } = null!;
    public IEnumerable<CategoryDto> Categories { get; set; } = null!;
    public IEnumerable<SizeDto> Sizes { get; set; } = null!;
    public IEnumerable<ProductSizeDto> ProductSizes { get; set; } = null!;
    public IEnumerable<DataSourceDto> DataSources { get; set; } = null!;
    public IEnumerable<MarketplaceDto> Marketplaces { get; set; } = null!;
    public IEnumerable<ProductDataSourceDto> ProductDataSources { get; set; } = null!;
    public IEnumerable<ProductMarketplaceDto> ProductMarketplaces { get; set; } = null!;
}
