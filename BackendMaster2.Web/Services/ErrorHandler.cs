using BackendMaster2.Web.Interfaces;
using BackendMaster2.Web.Models.Dtos;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace BackendMaster2.Web.Services;

public class ErrorHandler
{
    private readonly ISessionService _sessionService;
    private readonly NavigationManager _navigationManager;

    public ErrorHandler(ISessionService sessionService, NavigationManager navigationManager)
    {
        _sessionService = sessionService;
        _navigationManager = navigationManager;
    }

    public string HandleError<T>(ApiResult<T> result)
    {
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            _sessionService.DeleteAsync();
            _navigationManager.NavigateTo("/login");
            return "Sesión expirada. Por favor, inicia sesión de nuevo.";
        }

        if (result.Error?.Title == "Sin conexión con la API")
        {
            return "Sin conexión con la API. Comprueba que el backend esté arrancado.";
        }

        if ((int)result.StatusCode >= 500)
        {
            Console.WriteLine($"Error servidor {(int)result.StatusCode}: {result.Error?.Detail}");
            return "Error del servidor. Inténtalo de nuevo en unos minutos.";
        }

        return result.Error?.Detail ?? result.Error?.Title ?? $"Error {(int)result.StatusCode} al procesar la solicitud.";
    }
}


