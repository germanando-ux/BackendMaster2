namespace BackendMaster2.Shared.Domain;

//Atributos obligatorios por categoría
public class CategoryFeature
{
    // Clave compuesta
    public Guid CategoryId { get; set; }
    public Guid FeatureId { get; set; }

    // Payload de la relación
    public bool IsRequired { get; set; } = false;

    // Navegaciones
    public Category Category { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
}