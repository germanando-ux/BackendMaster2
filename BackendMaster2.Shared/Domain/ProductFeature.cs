namespace BackendMaster2.Shared.Domain;

public class ProductFeature
{
    // Clave compuesta
    public Guid ProductId { get; set; }
    public Guid FeatureId { get; set; }

    // Payload
    public string Value { get; set; } = string.Empty;
    public Guid? ChannelId { get; set; } // null = global, Guid = específico de canal

    // Navegaciones
    public Product Product { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
}