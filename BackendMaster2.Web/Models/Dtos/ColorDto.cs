using System.ComponentModel.DataAnnotations;

namespace BackendMaster2.Web.Models.Dtos;

public class ColorDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El código hexadecimal es obligatorio.")]
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; }
}
