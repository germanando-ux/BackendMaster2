
namespace BackendMaster2.Web.Models.Dtos;


/// <summary>
/// Espejo del ApiProblemDetails que devuelve el backend cuando algo falla.
/// </summary>
public class ApiProblemDetailsDto
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}