namespace BackendMaster2.Shared.Domain;

public class ProductFeature
{
    // 1. Clave subrogada (permite múltiples registros para la misma combinación ProductId + FeatureId)
    public Guid Id { get; set; }

    // 2. Claves foráneas
    public Guid ProductId { get; set; }
    public Guid FeatureId { get; set; }

    // 3. Payload
    public string? Value { get; set; }          
    public Guid? FeatureOptionId { get; set; }  
    public Guid? ChannelId { get; set; }        

    // 4. Navegaciones 
    public Product Product { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
    public FeatureOption? FeatureOption { get; set; } = null!;
}