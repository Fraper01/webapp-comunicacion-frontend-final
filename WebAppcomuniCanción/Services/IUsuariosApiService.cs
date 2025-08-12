using System.Threading.Tasks; // ¡Añade este 'using'!

namespace WebAppcomuniCancion.Interfaces
{
    public interface IUsuariosApiService
    {
        // Este método se usará para enviar las credenciales a la Web API
        Task<bool> AuthenticateUserAsync(string username, string password);

        // Si más adelante necesitas operaciones CRUD para usuarios desde la WebApp,
        // las firmas de esos métodos 
        // Task<List<UsuarioDto>> GetUsuariosAsync();
        // Task<UsuarioDto?> GetUsuarioByIdAsync(int id);
        // Task<bool> CreateUsuarioAsync(UsuarioDto usuario);
        // Task<bool> UpdateUsuarioAsync(UsuarioDto usuario);
        // Task<bool> DeleteUsuarioAsync(int id);
    }
}