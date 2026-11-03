using System.ComponentModel;

namespace BackendMaster2.Shared.Domain;

public class Feature
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public FeatureDataType DataType { get; set; }
    public bool IsRequired { get; set; } = false;

    // Navegaciones
    public ICollection<FeatureOption> FeatureOptions { get; set; } = new List<FeatureOption>();
    public ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>();
    public ICollection<CategoryFeature> CategoryFeatures { get; set; } = new List<CategoryFeature>();
}