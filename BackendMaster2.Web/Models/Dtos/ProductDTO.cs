using System.ComponentModel.DataAnnotations;

namespace BackendMaster2.Web.Models.Dtos;

/// <summary>
/// Espejo de la entidad Product del backend (BackendMaster2.Shared.Domain.Product).
/// Estrategia de espejo manual: si el contrato cambia, se actualizan los dos lados.
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "El SKU es obligatorio.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "El SKU solo admite mayúsculas, números y guiones.")]
    public string Sku { get; set; } = string.Empty;
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string? Description { get; set; }
    [Range(0, double.MaxValue, MinimumIsExclusive = true, ErrorMessage = "El precio debe ser superior a 0.")]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Guid> ColorIds { get; set; } = new();
}