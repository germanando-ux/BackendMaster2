using Microsoft.AspNetCore.Mvc;

namespace BackendMaster2.Api.Models;

/// <summary>
/// Extensión de ProblemDetails (RFC 7807) con metadata específica del proyecto.
/// Mantiene compatibilidad con herramientas estándar y añade información útil
/// para el frontend (OwnError, IsBdError).
/// </summary>
public class ApiProblemDetails : ProblemDetails
{
    /// <summary>
    /// Indica si el error se originó en nuestro código (true) o en el framework/infraestructura (false).
    /// Útil para que el frontend sepa si debe mostrar el mensaje tal cual o uno genérico.
    /// </summary>
    public bool OwnError { get; set; }

    /// <summary>
    /// Indica si el error proviene de la base de datos (violación de constraints, conexión, etc.).
    /// </summary>
    public bool IsBdError { get; set; }

    /// <summary>
    /// Stack trace visible solo en Development. En Production queda vacío.
    /// </summary>
    public string? StackTrace { get; set; }
}