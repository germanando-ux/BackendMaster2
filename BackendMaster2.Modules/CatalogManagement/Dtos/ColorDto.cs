namespace BackendMaster2.Api.Models.Dtos;

public class ºColorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; }
}
