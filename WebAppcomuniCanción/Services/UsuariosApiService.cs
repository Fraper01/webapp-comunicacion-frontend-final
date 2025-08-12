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

        public UsuariosApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")
                          ?? throw new ArgumentNullException("ApiSettings:BaseUrl", "La URL base de la API no está configurada en appsettings.json.");
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            var loginData = new { user = username, password = password };
            var jsonContent = JsonSerializer.Serialize(loginData); 
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"); 

            try
            {
                var response = await _httpClient.PostAsync("api/Usuarios/Authenticate", content);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP al autenticar usuario: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al autenticar usuario: {ex.Message}");
                return false;
            }
        }


    }
}