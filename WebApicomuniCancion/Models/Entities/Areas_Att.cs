using System.ComponentModel.DataAnnotations;

namespace WebApicomuniCancion.Models.Entities
{
    public class Areas_Att
    {
        [Key]
        public int Id_AreaAtt { get; set; }
        [Required(ErrorMessage = "El nombre del área es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del área no puede exceder los 100 caracteres.")]
        public string Area_Desarrollo { get; set; } = string.Empty;
        public DateTime? Fecha_Registro { get; set; }
        [StringLength(300, ErrorMessage = "La descripción del área no puede exceder los 300 caracteres.")]
        public string? Descipcion_Area { get; set; }
        public string? Usuario_Crea { get; set; }
        public string? Equipo_Crea { get; set; }
    }
}
