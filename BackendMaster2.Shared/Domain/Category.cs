namespace BackendMaster2.Shared.Domain;

public class Category 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Navegación: permite a EF Core saber que una Categoría tiene muchos ProductCategories
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}