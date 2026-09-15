using BackendMaster2.Api.Models;
using BackendMaster2.Shared.Common;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace BackendMaster2.Api.Middlewares;

/// <summary>
/// Middleware de manejo centralizado de errores HTTP.
/// Captura tanto excepciones no manejadas como status codes de error generados por el framework
/// (404 de routing, 400 de model binding) y los unifica en un formato ProblemDetails estándar.
/// 
/// Estrategia de buffering:
/// ASP.NET Core permite escribir en Response.Body una sola vez. Si el framework ya escribió
/// un error (por ejemplo, un 404 de ruta inexistente), no podemos sobreescribirlo directamente.
/// Por eso interceptamos el stream de salida con un MemoryStream temporal: si todo va bien,
/// volcamos el buffer al cliente; si hay error, descartamos el buffer y escribimos nuestra
/// respuesta de error formateada.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    // Opciones de serialización compartidas para evitar reinstanciar en cada request
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public ExceptionMiddleware(
        RequestDelegate next,
        IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Interceptamos el stream de salida: todo lo que se escriba irá al buffer, no al cliente todavía
        var originalBody = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Excepción no manejada: descartamos el buffer y escribimos nuestra respuesta de error
            context.Response.Body = originalBody;
            await WriteProblemDetailsAsync(context, ex);
            return;
        }

        // Si el framework generó un error sin excepción (404 de routing, 401 de [Authorize], etc.),
        // el StatusCode estará >= 400 pero el buffer estará vacío (no se escribió nada todavía).
        // Detectamos este caso y generamos nuestra respuesta formateada.
        if (context.Response.StatusCode >= 400 && buffer.Length == 0)
        {
            context.Response.Body = originalBody;
            await WriteProblemDetailsForStatusAsync(context);
            return;
        }

        // Éxito: volcamos el buffer al stream original para que el cliente reciba la respuesta normal
        buffer.Position = 0;
        await buffer.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }

    /// <summary>
    /// Mapea excepciones de dominio y del framework a respuestas ProblemDetails con metadata extendida.
    /// </summary>
    private async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
    {
        var problem = new ApiProblemDetails
        {
            Instance = context.Request.Path,
            OwnError = true,  // Error originado en nuestro código, no en infraestructura
            IsBdError = exception is PostgresException or NpgsqlException
        };

        // Mapeo de excepciones de dominio a códigos HTTP específicos
        switch (exception)
        {
            case NotFoundException:
                problem.Status = (int)HttpStatusCode.NotFound;
                problem.Title = "Recurso no encontrado";
                problem.Detail = exception.Message;
                problem.Type = "https://httpstatuses.com/404";
                break;

            case DuplicateResourceException:
                problem.Status = (int)HttpStatusCode.Conflict;
                problem.Title = "Recurso duplicado";
                problem.Detail = exception.Message;
                problem.Type = "https://httpstatuses.com/409";
                break;

            case ArgumentException:
                problem.Status = (int)HttpStatusCode.BadRequest;
                problem.Title = "Solicitud no válida";
                problem.Detail = exception.Message;
                problem.Type = "https://httpstatuses.com/400";
                break;

            case UnauthorizedAccessException:
                problem.Status = (int)HttpStatusCode.Unauthorized;
                problem.Title = "No autorizado";
                problem.Detail = exception.Message;
                problem.Type = "https://httpstatuses.com/401";
                break;

            default:
                // Error inesperado: en Development exponemos el mensaje real para debugging,
                // en Production devolvemos un mensaje genérico por seguridad
                problem.Status = (int)HttpStatusCode.InternalServerError;
                problem.Title = "Error interno del servidor";
                problem.Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "Se ha producido un error inesperado.";
                problem.Type = "https://httpstatuses.com/500";
                break;
        }

        // Stack trace completo solo en Development para facilitar debugging sin exponer internals en producción
        if (_environment.IsDevelopment())
        {
            problem.StackTrace = exception.ToString();
        }

        await WriteResponseAsync(context, problem);
    }

    /// <summary>
    /// Genera respuestas ProblemDetails para códigos HTTP de error sin excepción asociada.
    /// Estos casos ocurren cuando el framework rechaza la petición antes de llegar al controller
    /// (ruta inexistente, método no permitido, autenticación fallida, etc.).
    /// </summary>
    private async Task WriteProblemDetailsForStatusAsync(HttpContext context)
    {
        var statusCode = context.Response.StatusCode;

        var problem = new ApiProblemDetails
        {
            Status = statusCode,
            Instance = context.Request.Path,
            OwnError = false,  // Error generado por el framework, no por nuestro código
            IsBdError = false,
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = statusCode switch
            {
                400 => "Solicitud no válida",
                401 => "No autenticado",
                403 => "No autorizado",
                404 => $"No existe el recurso '{context.Request.Path}'",
                405 => "Método HTTP no permitido",
                409 => "Conflicto con el estado del recurso",
                415 => "Tipo de contenido no soportado",
                422 => "Error de validación",
                429 => "Demasiadas solicitudes",
                500 => "Error interno del servidor",
                503 => "Servicio no disponible",
                _ => $"Error HTTP {statusCode}"
            },
            Detail = statusCode switch
            {
                404 => $"La ruta '{context.Request.Path}' no corresponde a ningún endpoint.",
                405 => $"El método '{context.Request.Method}' no está permitido para este endpoint.",
                _ => null
            }
        };

        await WriteResponseAsync(context, problem);
    }

    private async Task WriteResponseAsync(HttpContext context, ApiProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problem, JsonOptions);
        await context.Response.WriteAsync(json);
    }
}