using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppcomuniCanción.Models
{
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Citas { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string? Nombre { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string? Apellido { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres.")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder los 15 caracteres.")]
        public string? Telefono { get; set; }

        [Display(Name = "Tipo Paciente")]
        [Required(ErrorMessage = "Debe seleccionar un tipo de paciente.")]
        [StringLength(20, ErrorMessage = "El tipo de paciente no puede exceder los 20 caracteres.")]
        public string? Tipo_Paciente { get; set; }

        [Display(Name = "Tipo Tratamiento")]
        [Required(ErrorMessage = "Debe seleccionar un tipo de tratamiento.")]
        [StringLength(50, ErrorMessage = "El tipo de tratamiento no puede exceder los 50 caracteres.")]
        public string? Tipo_Tratamiento { get; set; }
        public bool Turno_Manana { get; set; }
        public bool Turno_Tarde { get; set; }
        public bool Turno_Noche { get; set; }
        public bool Turno_Sabado { get; set; }
        public bool Desea_Alta_Usuario { get; set; }
        public bool Desea_Recibir_Novedades { get; set; }
        public DateTime? Fecha_Solicitud { get; set; }
        public string? Estatus { get; set; }
        public string EstatusSeleccionados =>
            (Turno_Manana ? "Pendiente, " : "") +
            (Turno_Tarde ? "Confirmada, " : "") +
            (Turno_Noche ? "Cancelada, " : "") +
            (Turno_Sabado ? "Completada, " : "");
        public string TurnosSeleccionados =>
            (Turno_Manana ? "Mañana, " : "") +
            (Turno_Tarde ? "Tarde, " : "") +
            (Turno_Noche ? "Noche, " : "") +
            (Turno_Sabado ? "Sábado, " : "");
    }
}
