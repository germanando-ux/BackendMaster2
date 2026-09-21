using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace BackendMaster2.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddMudServices();

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("Falta ApiBaseUrl en wwwroot/appsettings.json");

            // Cliente HTTP con nombre "Api" apuntando a tu API (URL en wwwroot/appsettings.json).
            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Registro de Servicio de sesión para guardar y recuperar tokens en localStorage.
            builder.Services.AddScoped<BackendMaster2.Web.Services.SessionService>();

            await builder.Build().RunAsync();


        }
    }
}
