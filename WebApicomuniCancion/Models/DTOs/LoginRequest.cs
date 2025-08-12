
    namespace WebApicomuniCancion.Models.DTOs
{
    // Este DTO (Data Transfer Object) se usará para recibir el usuario y la contraseña del cliente.
    public class LoginRequest
    {
        public string User { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty;
    }
}