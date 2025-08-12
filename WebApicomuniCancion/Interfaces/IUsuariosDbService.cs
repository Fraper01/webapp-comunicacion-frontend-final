using WebApicomuniCancion.Models.Entities;

namespace WebApicomuniCancion.Interfaces
{
    public interface IUsuariosDbService
    {
        Task<List<Usuarios>> GetAllUsuariosAsync();
        Task<Usuarios?> GetUsuarioByIdAsync(int id);
        Task AddUsuariosAsync(Usuarios usuarios);
        Task UpdateUsuariosAsync(Usuarios usuarios);
        Task DeleteUsuarioAsync(int id);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}
