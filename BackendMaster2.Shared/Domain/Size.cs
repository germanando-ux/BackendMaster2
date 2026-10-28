namespace BackendMaster2.Shared.Domain;

public class Size
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: "42", "M", "XL"
    public Guid SizeGroupId { get; set; }
    public bool IsActive { get; set; }

    // Navegaciones
    public SizeGroup SizeGroup { get; set; } = null!;
    public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}