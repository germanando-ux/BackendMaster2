using AutoMapper;
using BackendMaster2.Api.Hubs;
using BackendMaster2.Api.Infrastructure.Middlewares;
using BackendMaster2.Api.Infrastructure.Services;
using BackendMaster2.Api.Infrastructure.Settings;
using BackendMaster2.Api.Interface;
using BackendMaster2.Modules.Auth.Data;
using BackendMaster2.Modules.Auth.Interface;
using BackendMaster2.Modules.Data;
using BackendMaster2.Modules.Interfaces;
using BackendMaster2.Modules.Mappings;
using BackendMaster2.Modules.Repository;
using BackendMaster2.Modules.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace BackendMaster2.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ANTES del Build: registra los servicios de MVC (model binding, JSON, [ApiController]...)
            builder.Services.AddControllers();

            // Desactivamos la validación automática de [apicontroller] para que FluentValidation sea el único responsable
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Configurar la cadena de conexión a PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            //servicios de SignalR
            builder.Services.AddSignalR();

            //registro de inyección de dependencias 
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
            builder.Services.AddScoped<ICatalogService, CatalogService>();
            builder.Services.AddSingleton<IAiService, AiService>();
            //registro de servicios de notificación SignalR
            builder.Services.AddScoped<INotificationService, SignalRNotificationService>();


            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CatalogProfile).Assembly));

            // Registro de validadores de FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            // Registro de JWT
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Value,
                    ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Value!)),

                    RoleClaimType = "role"
                };
            });

            builder.Services.AddAuthorization();

            // Configuración de scallar/OpenAPI
            builder.Services.AddOpenApi();
            // Configuración de SignalR
            builder.Services.AddSignalR();

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontLocal", policy =>
                {
                    policy.WithOrigins("https://localhost:7275") // ← aquí tu origen real del front
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Configuración de Semantic Kernel
            builder.Services.AddKernel()
                .AddOpenAIChatCompletion(
                    modelId: builder.Configuration["AiSettings:ModelId"]!,
                    apiKey: builder.Configuration["AiSettings:ApiKey"]!,
                    endpoint: new Uri(builder.Configuration["AiSettings:Endpoint"]!)
                );
            var app = builder.Build();

            // 2. Mapear la ruta del Hub (junto a MapControllers)
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.UseHttpsRedirection(); //redirección de http a https
            app.UseCors("FrontLocal");

            //if (app.Environment.IsDevelopment())
            //{
            //    // Token fijo de desarrollo: misma cadena siempre, válido hasta fin de 2027.
            //    // Es un JWT real firmado con la clave de dev: el validador hace su trabajo
            //    // normal con él. En producción no vale nada (allí la clave es otra).
            //    var jwt = app.Services.GetRequiredService<IOptions<JwtSettings>>().Value;
            //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));
            //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //    var devToken = new JwtSecurityToken(
            //        issuer: jwt.Issuer,
            //        audience: jwt.Audience,
            //        claims: new[]
            //        {
            //new Claim(JwtRegisteredClaimNames.Sub, "admin@backendmaster2.com"),
            //new Claim("role", "Admin"),
            //new Claim(JwtRegisteredClaimNames.Jti, "dev-fixed-token")
            //        },
            //        notBefore: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            //        expires: new DateTime(2027, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            //        signingCredentials: creds);

            //    Console.WriteLine($"TOKEN FIJO DEV: {new JwtSecurityTokenHandler().WriteToken(devToken)}");
            //}

            //Middlewares 
            app.UseMiddleware<ExceptionMiddleware>();

            app.MapOpenApi();            // sirve el documento en /openapi/v1.json
            app.MapScalarApiReference(); // sirve la UI de Scalar en /scalar

            app.UseAuthentication();  // Primero: valida el token y rellena User
            app.UseAuthorization();   // Segundo: lee User.Claims y decide si pasa

            // DESPUÉS del Build: mapea las rutas de los attributes ([HttpGet], [Route]...) al pipeline
            app.MapControllers();


            app.Run();
        }
    }
}
