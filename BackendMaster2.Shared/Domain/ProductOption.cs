namespace BackendMaster2.Shared.Domain;

public class ProductOption
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid ColorId { get; set; }
    public Color Color { get; set; } = null!;
}
