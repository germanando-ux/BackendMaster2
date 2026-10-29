namespace BackendMaster2.Modules.Dtos;

public class ProductMarketplaceDto
{
    public Guid ProductId { get; set; }
    public Guid MarketplaceId { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public DateTime? LastSyncedAt { get; set; }
}