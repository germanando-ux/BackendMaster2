
namespace BackendMaster2.Web.Models.Dtos;

public class FeatureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public FeatureDataType DataType { get; set; }
    public bool IsRequired { get; set; }

    // Solo se da la relación si datatype está informado
    public ICollection<FeatureOptionDto> Options { get; set; } = new List<FeatureOptionDto>();
}