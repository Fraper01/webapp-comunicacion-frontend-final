using System.Text.Json;
using WebAppcomuniCanción.Models;
using System.Net.Http; 
using System.Text; 
using System; 
using Microsoft.Extensions.Configuration; 

namespace WebAppcomuniCanción.Services
{
    public class AreasApiService : IAreasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public AreasApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")
                          ?? throw new ArgumentNullException("ApiSettings:BaseUrl", "La URL base de la API no está configurada en appsettings.json.");
            _httpClient.BaseAddress = new Uri(_baseApiUrl); 
        }

        public async Task<List<AreaDto>> GetAreasAsync()
        {
            var response = await _httpClient.GetAsync("api/Areas"); 
            response.EnsureSuccessStatusCode(); 

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var areas = JsonSerializer.Deserialize<List<AreaDto>>(content, options);

            return areas ?? new List<AreaDto>(); 
        }

        public async Task<AreaDto?> GetAreaByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Areas/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var area = JsonSerializer.Deserialize<AreaDto>(content, options);

                return area;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener área por ID: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener área por ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAreaAsync(AreaDto area)
        {
            area.Fecha_Registro = DateTime.Now;
            area.Equipo_Crea = Environment.MachineName; 
            area.Usuario_Crea = "Francisco"; 

            var jsonContent = JsonSerializer.Serialize(area);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Areas", content);

                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al crear área: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAreaAsync(AreaDto area)
        {
            if (area.Id_Area == 0) 
            {
                Console.WriteLine("Error: El Identificador del Area es necesario para actualizar un registro.");
                return false;
            }

            area.Fecha_Registro = DateTime.Now;
            area.Equipo_Crea = Environment.MachineName;
            area.Usuario_Crea = "Francisco Modifica";

            var jsonContent = JsonSerializer.Serialize(area);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PutAsync($"api/Areas/{area.Id_Area}", content);

                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al actualizar área: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al actualizar área: {ex.Message}");
                return false;
            }
        }
    }
}


