using WebAppcomuniCanción.Models;

namespace WebAppcomuniCanción.Services
{
    public interface ICitasApiService
    {
        Task<List<CitaDto>?> GetCitasAsync();
        Task<CitaDto?> GetCitaByIdAsync(int id);
        Task<bool> CreateCitaAsync(CitaDto cita);
        Task<bool> UpdateCitaAsync(CitaDto cita);
        Task<bool> UpdateCitaStatusAsync(int id, string newStatus);

    }
}
