namespace BackendMaster2.Modules.Dtos;

public class ColorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
