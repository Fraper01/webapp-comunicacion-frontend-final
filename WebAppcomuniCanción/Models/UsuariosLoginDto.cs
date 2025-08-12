using System.ComponentModel.DataAnnotations; // ¡Añade este 'using'!

namespace WebAppcomuniCancion.Models
{
    public class UsuariosLoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Usuario")]
        public string User { get; set; } = string.Empty; 

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty; 
    }
}