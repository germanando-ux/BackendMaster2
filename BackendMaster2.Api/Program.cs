using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.ProductManagement.Data;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Modules.ProductManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace BackendMaster2.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);           

            // Configurar la cadena de conexión a PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            //registro de inyección de dependencias 
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
