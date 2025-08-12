using WebApicomuniCancion.Services;
using Microsoft.Extensions.Configuration; // Ya lo tienes, pero es necesario para IConfiguration
using WebApicomuniCancion.Interfaces;
using Microsoft.AspNetCore.Builder; // Asegúrate de tener este using para WebApplicationBuilder y WebApplication
using Microsoft.Extensions.DependencyInjection; // Asegúrate de tener este using para AddCors

var builder = WebApplication.CreateBuilder(args);

// Acceder a la configuración de CorsOrigins desde appsettings.json
// Asegúrate de que appsettings.json tiene la sección "CorsOrigins" con "Allowed"
var corsOrigins = builder.Configuration.GetSection("CorsOrigins:Allowed").Get<string[]>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// *** INICIO DE LA CONFIGURACIÓN DEL SERVICIO CORS ***
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebAppAccess", // Nombre de tu política CORS
        policy =>
        {
            if (corsOrigins != null && corsOrigins.Length > 0)
            {
                policy.WithOrigins(corsOrigins) // Permite los orígenes definidos en appsettings.json
                      .AllowAnyHeader()       // Permite cualquier tipo de encabezado
                      .AllowAnyMethod();      // Permite cualquier método HTTP (GET, POST, PUT, DELETE)
                // .AllowCredentials(); // Descomentar si tu cliente necesita enviar cookies o credenciales (raro en tu caso inicial)
            }
            else
            {
                // Fallback para desarrollo si no hay orígenes configurados.
                // ¡ADVERTENCIA: NO USAR AllowAnyOrigin() en PRODUCCIÓN sin control estricto!
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                Console.WriteLine("Advertencia: No se encontraron orígenes CORS configurados en appsettings.json para 'CorsOrigins:Allowed'. Permitiendo cualquier origen (solo para desarrollo/debug).");
            }
        });
});
// *** FIN DE LA CONFIGURACIÓN DEL SERVICIO CORS ***


builder.Services.AddScoped<IAreasDbService, AreasDbService>();
builder.Services.AddScoped<IAreasAttDbService, AreasAttDbService>();
builder.Services.AddScoped<IUsuariosDbService, UsuariosDbService>();
builder.Services.AddScoped<ICitasDbService, CitasDbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Generalmente se añade aquí si usas HTTPS

// *** HABILITAR EL MIDDLEWARE CORS EN EL PIPELINE DE SOLICITUDES ***
// Asegúrate de que esto esté ANTES de UseAuthorization() y MapControllers()
app.UseCors("AllowWebAppAccess"); // Usa el mismo nombre de política que definiste arriba
// *** FIN DE HABILITAR EL MIDDLEWARE CORS ***

app.UseAuthorization();

app.MapControllers();

app.Run();