using System.Collections.Generic;
using System.Threading.Tasks;
using WebApicomuniCancion.Models.Entities;

namespace WebApicomuniCancion.Services
{
    public interface IAreasDbService
    {
        Task<List<Area>> GetAllAreasAsync();
        Task<Area?> GetAreaByIdAsync(int id);
        Task AddAreaAsync(Area area);
        Task UpdateAreaAsync(Area area);
        Task DeleteAreaAsync(int id);
    }
}