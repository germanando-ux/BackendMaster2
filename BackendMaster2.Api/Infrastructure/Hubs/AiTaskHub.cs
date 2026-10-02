using Microsoft.AspNetCore.SignalR;

namespace BackendMaster2.Api.Hubs;

public class AiTaskHub : Hub
{
    public async Task Connect()
    {
        await Clients.Caller.SendAsync("Connected", "Conexión establecida con el Hub de IA.");
    }
}