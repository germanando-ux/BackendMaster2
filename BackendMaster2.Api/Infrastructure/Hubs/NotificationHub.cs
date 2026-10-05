using Microsoft.AspNetCore.SignalR;

namespace BackendMaster2.Api.Hubs;

public class NotificationHub : Hub
{
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", "Conexión activa con el NotificationHub.");
    }
}