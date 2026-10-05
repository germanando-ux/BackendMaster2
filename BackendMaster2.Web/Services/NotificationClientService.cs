using BackendMaster2.Web.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace BackendMaster2.Web.Services;

public class NotificationClientService : INotificationClientService
{
    private readonly HubConnection _hubConnection;
    private readonly string _apiBaseUrl;
    private bool _isStarted;

    // Ajusta esta URL a la URL real de tu API (ej: "https://localhost:7001")

    public event Action<string, string>? OnTaskCompleted;
    public event Action<string, string>? OnTaskFailed;

    public NotificationClientService(IConfiguration configuration)
    {

        _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")
                      ?? "https://localhost:7001";

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_apiBaseUrl}/hubs/notifications")
            .WithAutomaticReconnect()
            .Build();

        // Suscribirse a los eventos que emite el backend
        _hubConnection.On<string, string>("OnTaskCompleted", (taskId, result) =>
        {
            OnTaskCompleted?.Invoke(taskId, result);
        });

        _hubConnection.On<string, string>("OnTaskFailed", (taskId, error) =>
        {
            OnTaskFailed?.Invoke(taskId, error);
        });
    }

    /// <summary>
    /// Inicia la conexión con el Hub de notificaciones.
    /// </summary>
    public async Task StartAsync()
    {
        if (_isStarted) return;

        try
        {
            await _hubConnection.StartAsync();
            _isStarted = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al conectar con SignalR: {ex.Message}");
        }
    }

    public string? GetConnectionId()
    {
        return _hubConnection.ConnectionId;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isStarted)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _isStarted = false;
        }
    }
}