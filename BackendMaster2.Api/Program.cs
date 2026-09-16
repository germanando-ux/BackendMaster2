using BackendMaster2.Api.Middlewares;
using BackendMaster2.Api.Settings;
using BackendMaster2.Modules.Auth.Data;
using BackendMaster2.Modules.Auth.Interface;
using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.ProductManagement.Data;
using BackendMaster2.Modules.ProductManagement.Interface;
using BackendMaster2.Modules.ProductManagement.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace BackendMaster2.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ANTES del Build: registra los servicios de MVC (model binding, JSON, [ApiController]...)
            builder.Services.AddControllers();

            // Configurar la cadena de conexión a PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            //registro de inyección de dependencias 
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Registro de validadores de FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Registro de JWT
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

            // Configuración de scallar/OpenAPI
            builder.Services.AddOpenApi();

            var app = builder.Build();

            //Middlewares 
            app.UseMiddleware<ExceptionMiddleware>();

            app.MapOpenApi();            // sirve el documento en /openapi/v1.json
            app.MapScalarApiReference(); // sirve la UI de Scalar en /scalar

            // DESPUÉS del Build: mapea las rutas de los attributes ([HttpGet], [Route]...) al pipeline
            app.MapControllers();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
