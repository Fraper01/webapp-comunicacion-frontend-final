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
            _httpClient.BaseAddress = new Uri(_baseApiUrl); // Establece la base para HttpClient
        }

        public async Task<List<AreaDto>> GetAreasAsync()
        {
            var response = await _httpClient.GetAsync("api/Areas"); // Asume que tu endpoint es /api/Areas
            response.EnsureSuccessStatusCode(); // Lanza una excepción si el código de estado no es de éxito

            var content = await response.Content.ReadAsStringAsync();
            // Opciones para deserializar JSON, por ejemplo, ignorar mayúsculas/minúsculas
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var areas = JsonSerializer.Deserialize<List<AreaDto>>(content, options);

            return areas ?? new List<AreaDto>(); // Devuelve la lista o una lista vacía si es null
        }

        public async Task<AreaDto?> GetAreaByIdAsync(int id)
        {
            try
            {
                // Construye la URL para obtener un área específica por su ID
                var response = await _httpClient.GetAsync($"api/Areas/{id}");

                // Si la respuesta no es de éxito (ej. 404 Not Found), GetAreaByIdAsync debería devolver null
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                // Lanza una excepción si el código de estado no es de éxito (para otros errores 4xx/5xx)
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var area = JsonSerializer.Deserialize<AreaDto>(content, options);

                return area;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener área por ID: {ex.Message}");
                // Puedes loguear el error o relanzar la excepción si es un error grave
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

            // Serializa el objeto AreaDto a una cadena JSON
            var jsonContent = JsonSerializer.Serialize(area);

            // Crea un HttpContent con el JSON, especificando que es JSON
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                // Envía la solicitud POST al endpoint de creación de áreas (ej. /api/Areas)
                var response = await _httpClient.PostAsync("api/Areas", content);

                // Verifica si la solicitud fue exitosa (códigos 2xx)
                response.EnsureSuccessStatusCode();

                // Si la API devuelve el objeto creado, puedes deserializarlo y devolverlo
                // var createdArea = JsonSerializer.Deserialize<AreaDto>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                // return createdArea;

                // Si solo necesitamos saber si fue exitoso, devolvemos true
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores en caso de fallo de la solicitud HTTP
                Console.WriteLine($"Error al crear área: {ex.Message}");
                // Puedes loguear el error o lanzar una excepción más específica
                return false;
            }
            catch (Exception ex)
            {
                // Otros posibles errores (ej. de serialización/deserialización)
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
                // Envía la solicitud PUT a la ruta específica del recurso (ej. /api/Areas/{id})
                var response = await _httpClient.PutAsync($"api/Areas/{area.Id_Area}", content);

                // Verifica si la solicitud fue exitosa (códigos 2xx)
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al actualizar área: {ex.Message}");
                // Puedes loguear el error o lanzar una excepción más específica
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


