namespace BackendMaster2.Web.Models.Dtos;

public class SizeGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}