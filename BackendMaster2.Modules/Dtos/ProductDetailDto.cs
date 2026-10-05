namespace BackendMaster2.Modules.Dtos;

public class ProductDetailDto
{    
    public ProductDto Product { get; set; } = null!;
    public IEnumerable<ColorDto> Colors { get; set; } = null!;
}
