namespace BackendMaster2.Modules.Dtos;

public class ProductDataSourceDto
{
    public Guid ProductId { get; set; }
    public Guid DataSourceId { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public DateTime? LastSyncedAt { get; set; }
}