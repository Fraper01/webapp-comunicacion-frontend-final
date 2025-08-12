using System.ComponentModel.DataAnnotations;

namespace WebApicomuniCancion.Models.Entities
{
    public class Usuarios
    {
        [Key]
        public int id_user { get; set; }
        [Required(ErrorMessage = "El nombre del usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del usuario no puede exceder los 100 caracteres.")]
        public string full_name { get; set; } = string.Empty;
        [Required(ErrorMessage = "El codigo del usuario es obligatorio.")]
        [StringLength(20, ErrorMessage = "El codigo del usuario no puede exceder los 20 caracteres.")]
        public string user { get; set; } = string.Empty;
        [Required(ErrorMessage = "El password del usuario es obligatorio.")]
        [StringLength(20, ErrorMessage = "El password del usuario no puede exceder los 20 caracteres.")]
        public string password { get; set; } = string.Empty;
        public DateTime? fecha_registro { get; set; }
        [StringLength(300, ErrorMessage = "La descripción del área no puede exceder los 300 caracteres.")]
        public string? Usuario_Crea { get; set; }
        public string? Equipo_Crea { get; set; }
    }
}
    

