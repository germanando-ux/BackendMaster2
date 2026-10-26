namespace BackendMaster2.Web.Models.Dtos;

/// <summary>
/// Cajón de sastre para DTOs ad-hoc del frontend (espejo del backend).
/// </summary>
public class ProductSearchHybridDto
{
    public ProductDto Product { get; set; } = new();
    public double SimilarityScore { get; set; }
}