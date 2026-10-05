namespace BackendMaster2.Web.Models.Dtos;

public class EnrichDescriptionRequestDto
{
    public string CurrentDescription { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new();
    public decimal Price { get; set; }
    public string SRConnectionId { get; set; } = string.Empty;

}
