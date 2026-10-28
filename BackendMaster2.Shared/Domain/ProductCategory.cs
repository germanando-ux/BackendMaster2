namespace BackendMaster2.Shared.Domain;

public class ProductCategory
{
    // Clave compuesta (PK y FK a la vez)
    public Guid ProductId { get; set; }
    public Guid CategoryId { get; set; }

    // Propiedad de negocio
    public bool IsPrimary { get; set; }

    // Navegaciones (para EF Core)
    public Product Product { get; set; } = null!;
    public Category Category { get; set; } = null!;
}