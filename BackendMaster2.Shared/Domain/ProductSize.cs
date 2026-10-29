namespace BackendMaster2.Shared.Domain;

public class ProductSize
{
    // Clave compuesta (PK + FK)
    public Guid ProductId { get; set; }
    public Guid SizeId { get; set; }

    // Payload (datos propios de la relación)
    public int Stock { get; set; }
    public string? Sku { get; set; } // SKU específico de esta variante (opcional)
    public decimal? Price { get; set; }

    // Navegaciones
    public Product Product { get; set; } = null!;
    public Size Size { get; set; } = null!;
}