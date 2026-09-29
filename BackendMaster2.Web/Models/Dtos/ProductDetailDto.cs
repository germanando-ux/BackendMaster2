namespace BackendMaster2.Web.Models.Dtos;

public class ProductDetailDto
{
    public ProductDto Product { get; set; } = null!;
    public IEnumerable<ColorDto> Colors { get; set; } = null!;
}
