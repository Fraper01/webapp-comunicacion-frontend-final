using WebAppcomuniCanción.Models;

namespace WebAppcomuniCanción.Services
{
    public interface IAreasApiService
    {
        Task<List<AreaDto>> GetAreasAsync();
        Task<AreaDto?> GetAreaByIdAsync(int id);
        Task<bool> CreateAreaAsync(AreaDto area);
        Task<bool> UpdateAreaAsync(AreaDto area);
    }
}
