using WebApicomuniCancion.Models.Entities;

namespace WebApicomuniCancion.Interfaces
{
    public interface IAreasAttDbService
    {
        Task<List<Areas_Att>> GetAllAreasAttAsync();
        Task<Areas_Att?> GetAreaAttByIdAsync(int id);
        Task AddAreaAttAsync(Areas_Att areas_att);
        Task UpdateAreaAttAsync(Areas_Att areas_att);
        Task DeleteAreaAttAsync(int id);
    }
}
