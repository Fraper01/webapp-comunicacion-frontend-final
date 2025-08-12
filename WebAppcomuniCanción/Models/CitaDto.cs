using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppcomuniCanción.Models
{
    public class CitaDto
    {

        public int Id_Citas { get; set; }

        public string? Nombre { get; set; } 

        public string? Apellido { get; set; } 

        public string? Email { get; set; } 

        public string? Telefono { get; set; } 

        public string? Tipo_Paciente { get; set; } 

        public string? Tipo_Tratamiento { get; set; } 
        public bool Turno_Manana { get; set; }
        public bool Turno_Tarde { get; set; }
        public bool Turno_Noche { get; set; }
        public bool Turno_Sabado { get; set; }
        public bool Desea_Alta_Usuario { get; set; }
        public bool Desea_Recibir_Novedades { get; set; }
        public DateTime? Fecha_Solicitud { get; set; }
        public string? Estatus { get; set; }

        public string TurnosSeleccionados =>
            (Turno_Manana ? "Mañana, " : "") +
            (Turno_Tarde ? "Tarde, " : "") +
            (Turno_Noche ? "Noche, " : "") +
            (Turno_Sabado ? "Sábado, " : "");
    }
}
