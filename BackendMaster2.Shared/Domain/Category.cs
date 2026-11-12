namespace BackendMaster2.Shared.Domain;

public class Category 
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    

    // Navegación: permite a EF Core saber que una Categoría tiene muchos ProductCategories
    public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    //navegación: permite a EF Core saber que una Categoría tiene muchos CategoryFeatures
    public ICollection<CategoryFeature> CategoryFeatures { get; set; } = new List<CategoryFeature>();

    //NAVEGACIONES PARA LA JERARQUIA DE CATEGORIAS

    // Navegación hacia el padre (nullable porque las raíces no tienen padre)
    public Category? Parent { get; set; }

    // Navegación hacia los hijos (colección, inicializada para evitar nulls)
    public ICollection<Category> Children { get; set; } = new List<Category>();

}