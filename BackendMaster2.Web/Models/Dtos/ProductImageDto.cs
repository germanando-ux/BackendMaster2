namespace BackendMaster2.Web.Models.Dtos;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int Order { get; set; }
    public bool IsPrimary { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public DateTime CreatedAt { get; set; }
}
