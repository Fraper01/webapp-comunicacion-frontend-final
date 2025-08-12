using WebAppcomuniCancion.Interfaces;
using WebAppcomuniCancion.Models; 
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebAppcomuniCanción.Models;
using System;
using System.Collections.Generic; 
using System.Net.Http.Json; 

namespace WebAppcomuniCanción.Services
{
    public class CitasApiService : ICitasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public CitasApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration.GetValue<string>("ApiSettings:BaseUrl")
                          ?? throw new ArgumentNullException("ApiSettings:BaseUrl", "La URL base de la API no está configurada en appsettings.json.");
            _httpClient.BaseAddress = new Uri(_baseApiUrl); 
        }

        public async Task<bool> CreateCitaAsync(CitaDto citaDto)
        {
            citaDto.Fecha_Solicitud = DateTime.Now;

            var jsonContent = JsonSerializer.Serialize(citaDto);

            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/Citas", httpContent);

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
                Console.WriteLine($"Error inesperado al crear cita: {ex.Message}");
                return false;
            }
        }

        public async Task<CitaDto?> GetCitaByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Citas/{id}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<CitaDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al obtener cita por ID {id}: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error de JSON al obtener cita por ID {id}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener cita por ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CitaDto>?> GetCitasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Citas");
                response.EnsureSuccessStatusCode(); 

                return await response.Content.ReadFromJsonAsync<List<CitaDto>>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al obtener citas: {ex.Message}");
                return null;
            }
            catch (JsonException ex) 
            {
                Console.WriteLine($"Error de JSON al obtener citas: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al obtener citas: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateCitaAsync(CitaDto cita)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Citas/{cita.Id_Citas}", cita);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al actualizar cita {cita.Id_Citas}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al actualizar cita {cita.Id_Citas}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCitaStatusAsync(int id, string newStatus)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Citas/{id}/estatus", newStatus);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error HTTP al actualizar estatus de cita {id}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al actualizar estatus de cita {id}: {ex.Message}");
                return false;
            }
        }

    }
}
