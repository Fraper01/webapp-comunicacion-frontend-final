using System.Threading.Tasks; 

namespace WebAppcomuniCancion.Interfaces
{
    public interface IUsuariosApiService
    {
        Task<bool> AuthenticateUserAsync(string username, string password);

        // Task<List<UsuarioDto>> GetUsuariosAsync();
        // Task<UsuarioDto?> GetUsuarioByIdAsync(int id);
        // Task<bool> CreateUsuarioAsync(UsuarioDto usuario);
        // Task<bool> UpdateUsuarioAsync(UsuarioDto usuario);
        // Task<bool> DeleteUsuarioAsync(int id);
    }
}