namespace BackendMaster2.Web.Models.Dtos;

/// <summary>
/// Espejo de la entidad Product del backend (BackendMaster2.Shared.Domain.Product).
/// Estrategia de espejo manual: si el contrato cambia, se actualizan los dos lados.
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
}