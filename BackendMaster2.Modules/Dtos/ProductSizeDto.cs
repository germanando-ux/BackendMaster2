namespace BackendMaster2.Modules.Dtos;

public class ProductSizeDto
{
    public Guid ProductId { get; set; }
    public Guid SizeId { get; set; }
    public int Stock { get; set; }
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
}