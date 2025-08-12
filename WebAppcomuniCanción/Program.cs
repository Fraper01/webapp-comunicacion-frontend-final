using Microsoft.Extensions.Configuration;
using WebAppcomuniCancion.Interfaces;

// Usings necesarios para la sesión
using Microsoft.AspNetCore.Http;
using WebAppcomuniCanción.Services;
using WebAppcomuniCancion.Services;


var builder = WebApplication.CreateBuilder(args);

// Si tu WebAPI usa 5000/7000, entonces 5001/7001 para la WebApp evitar el conflicto.
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

// Esto es necesario para que las sesiones funcionen. MemoryCache es para desarrollo.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});


var app = builder.Build();
//app.Urls.Add("http://localhost:5001");
//app.Urls.Add("https://localhost:7001"); 


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