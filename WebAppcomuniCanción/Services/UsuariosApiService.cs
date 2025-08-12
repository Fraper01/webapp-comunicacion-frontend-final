using System.Text.Json; 
using WebAppcomuniCancion.Models; 
using System.Net.Http; 
using System.Text; 
using System; 
using Microsoft.Extensions.Configuration; 
using System.Threading.Tasks; 
using WebAppcomuniCancion.Interfaces; 

namespace WebAppcomuniCancion.Services
{
    public class UsuariosApiService : IUsuariosApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        // El constructor recibe HttpClient (inyectado por el sistema) y IConfiguration
        public UsuariosApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Obtenemos la URL base de la API desde la configuración (appsettings.json)
            _baseApiUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")
                          ?? throw new ArgumentNullException("ApiSettings:BaseUrl", "La URL base de la API no está configurada en appsettings.json.");
            // Establecemos la URL base en el HttpClient para que las rutas relativas funcionen
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
        }

        // Método para autenticar al usuario
        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            var loginData = new { user = username, password = password };
            var jsonContent = JsonSerializer.Serialize(loginData); // Serializamos a JSON
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"); // Creamos el contenido HTTP

            try
            {
                // Enviamos la solicitud POST al endpoint de autenticación en tu Web API
                // La URL completa será _baseApiUrl + "api/Usuarios/Authenticate"
                var response = await _httpClient.PostAsync("api/Usuarios/Authenticate", content);

                // Si la API devuelve un código de estado de éxito (2xx), consideramos que la autenticación fue exitosa
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores de red o HTTP
                Console.WriteLine($"Error de solicitud HTTP al autenticar usuario: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Otros errores inesperados (ej. problemas de serialización/deserialización si hubiera respuesta JSON)
                Console.WriteLine($"Error inesperado al autenticar usuario: {ex.Message}");
                return false;
            }
        }

        // añadir otros métodos para interactuar con la API de Usuarios,

    }
}