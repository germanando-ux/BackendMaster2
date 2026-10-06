using BackendMaster2.Api.Hubs;
using BackendMaster2.Modules.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BackendMaster2.Api.Infrastructure.Services;


/// <summary>
/// Envía avisos en tiempo real mediante SignalR al cliente que hizo la petición.
/// </summary>
/// <remarks>
/// Actúa como puente para que el módulo de negocio pueda emitir eventos sin
/// acoplarse ni depender de SignalR o de la API.
/// </remarks>

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    
    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyClientAsync(string connectionId, string method, string description, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            return;
        }        
        await _hubContext.Clients.Client(connectionId).SendAsync(method, description, cancellationToken);

    }
}