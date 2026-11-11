using System.ComponentModel.DataAnnotations;

namespace BackendMaster2.Web.Models.Dtos;

public class ProductSizeDto
{
    public Guid ProductId { get; set; }
    public Guid SizeId { get; set; }
    public string SizeName { get; set; } = string.Empty;
    public string SizeGroupName { get; set; } = string.Empty;
    public int Stock { get; set; }
    [Required(ErrorMessage = "El SKU es obligatorio.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "El SKU solo admite mayúsculas, números y guiones.")]
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
}