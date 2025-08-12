using WebApicomuniCancion.Models.Entities;

namespace WebApicomuniCancion.Models.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public Usuarios? UserData { get; set; } 
    }
}
