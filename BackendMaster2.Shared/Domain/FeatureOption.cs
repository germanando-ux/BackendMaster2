namespace BackendMaster2.Shared.Domain;

//Valores predefinidos para atributos de tipo Lista
public class FeatureOption
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }
    public string Value { get; set; } = string.Empty;

    // Navegación
    public Feature Feature { get; set; } = null!;
}