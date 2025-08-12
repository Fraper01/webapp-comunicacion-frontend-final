using Microsoft.Extensions.Configuration;
using WebAppcomuniCancion.Interfaces;

// Usings necesarios para la sesión
using Microsoft.AspNetCore.Http;
using WebAppcomuniCanción.Services;
using WebAppcomuniCancion.Services;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5001", "https://localhost:7001");

//Add services to the container.
builder.Services.AddControllersWithViews();

// Configura HttpClient y registra tu servicio
builder.Services.AddHttpClient<IAreasApiService, AreasApiService>(client =>
{
#pragma warning disable CS8604 // Posible argumento de referencia nulo
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:BaseUrl"));
#pragma warning restore CS8604 // Posible argumento de referencia nulo
});
builder.Services.AddHttpClient<IUsuariosApiService, UsuariosApiService>(client =>
{
#pragma warning disable CS8604 // Posible argumento de referencia nulo
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:BaseUrl"));
#pragma warning restore CS8604 // Posible argumento de referencia nulo
});
builder.Services.AddHttpClient<ICitasApiService, CitasApiService>(client =>
{
#pragma warning disable CS8604 // Posible argumento de referencia nulo
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:BaseUrl"));
#pragma warning restore CS8604 // Posible argumento de referencia nulo
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); 
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 

app.UseAuthorization(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();