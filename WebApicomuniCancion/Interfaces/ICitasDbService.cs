using WebApicomuniCancion.Models.Entities;

namespace WebApicomuniCancion.Interfaces
{
    public interface ICitasDbService
    {
        Task<List<Cita>> GetAllCitasAsync();
        Task<Cita?> GetCitaByIdAsync(int id);
        Task AddCitaAsync(Cita cita);
        Task UpdateCitaAsync(Cita cita);
        Task DeleteCitaAsync(int id);
        Task<bool> UpdateCitaStatusAsync(int id, string newStatus);
    }
}
