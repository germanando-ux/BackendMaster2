namespace BackendMaster2.Modules.Dtos;

public class SizeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid SizeGroupId { get; set; }
    public bool IsActive { get; set; }
}