using WebApicomuniCancion.Services;
using Microsoft.Extensions.Configuration; 
using WebApicomuniCancion.Interfaces;
using Microsoft.AspNetCore.Builder; 
using Microsoft.Extensions.DependencyInjection; 

var builder = WebApplication.CreateBuilder(args);

// Acceder a la configuración de CorsOrigins desde appsettings.json
var corsOrigins = builder.Configuration.GetSection("CorsOrigins:Allowed").Get<string[]>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebAppAccess", 
        policy =>
        {
            if (corsOrigins != null && corsOrigins.Length > 0)
            {
                policy.WithOrigins(corsOrigins) 
                      .AllowAnyHeader()       
                      .AllowAnyMethod();     
            }
            else
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                Console.WriteLine("Advertencia: No se encontraron orígenes CORS configurados en appsettings.json para 'CorsOrigins:Allowed'. Permitiendo cualquier origen (solo para desarrollo/debug).");
            }
        });
});


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

// *** HABILITAR EL MIDDLEWARE CORS EN EL PIPELINE DE SOLICITUDES ***
app.UseCors("AllowWebAppAccess"); 

app.UseAuthorization();

app.MapControllers();

app.Run();