namespace BackendMaster2.Shared.Domain;

public class SizeGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ej: "EU", "US", "INT"
    public bool IsActive { get; set; }

    // Navegación: un SizeGroup tiene muchas tallas
    public ICollection<Size> Sizes { get; set; } = new List<Size>();
}