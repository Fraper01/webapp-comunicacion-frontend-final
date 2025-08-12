using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApicomuniCancion.Interfaces;
using WebApicomuniCancion.Models.Entities;
using WebApicomuniCancion.Services;

namespace WebApicomuniCancion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ILogger<CitasController> _logger;

        private readonly ICitasDbService _citasDbService; 

        public CitasController(ICitasDbService citasDbService, ILogger<CitasController> logger)
        {
            _citasDbService = citasDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            try
            {
                var citas = await _citasDbService.GetAllCitasAsync();
                return Ok(citas); 
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error en GET /api/Citas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); 
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            try
            {
                var cita = await _citasDbService.GetCitaByIdAsync(id);

                if (cita == null) 
                {
                    return NotFound($"Área con ID {id} no encontrada."); 
                }

                return Ok(cita); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/Citas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita([FromBody] Cita cita)
        {
            if (string.IsNullOrWhiteSpace(cita.Nombre))
            {
                return BadRequest("El nombre de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Apellido))
            {
                return BadRequest("El apellido de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Email))
            {
                return BadRequest("El email de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Paciente))
            {
                return BadRequest("El tipo paciente de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Tratamiento))
            {
                return BadRequest("El tipo tratamiento de la cita es obligatorio."); 
            }
            if (string.IsNullOrEmpty(cita.Estatus)) 
            {
                cita.Estatus = "Pendiente";
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _citasDbService.AddCitaAsync(cita);
                return Ok(cita);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/Citas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, [FromBody] Cita cita)
        {
            if (id != cita.Id_Citas)
            {
                return BadRequest("El ID de la URL no coincide con el ID del citas en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(cita.Nombre)) 
            {
                return BadRequest("El nombre del cita es obligatorio para la actualización.");
            }
            if (string.IsNullOrWhiteSpace(cita.Apellido))
            {
                return BadRequest("El apellido de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Email))
            {
                return BadRequest("El email de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Paciente))
            {
                return BadRequest("El tipo paciente de la cita es obligatorio."); 
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Tratamiento))
            {
                return BadRequest("El tipo tratamiento de la cita es obligatorio."); 
            }
            if (string.IsNullOrEmpty(cita.Estatus))
            {
                cita.Estatus = "Pendiente";
            }
            string[] allowedStatuses = { "Pendiente", "Confirmada", "Cancelada", "Completada" };
            if (!allowedStatuses.Contains(cita.Estatus))
            {
                return BadRequest($"El estatus '{cita.Estatus}' no es válido.");
            }

            try
            {
                var existingCita = await _citasDbService.GetCitaByIdAsync(id);
                if (existingCita == null)
                {
                    return NotFound($"Citas con ID {id} no encontrada para actualizar.");
                }

                await _citasDbService.UpdateCitaAsync(cita);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en PUT /api/Citas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al actualizar el dato.");
            }
        }

        [HttpPut("{id}/estatus")] 
        public async Task<IActionResult> UpdateCitaEstatus(int id, [FromBody] string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                return BadRequest("El nuevo estatus no puede estar vacío.");
            }

            var existingCita = await _citasDbService.GetCitaByIdAsync(id);

            if (existingCita == null)
            {
                return NotFound();
            }

            string[] allowedStatuses = { "Pendiente", "Confirmada", "Cancelada", "Completada" };
            if (!allowedStatuses.Contains(newStatus))
            {
                return BadRequest($"El estatus '{newStatus}' no es válido.");
            }

            existingCita.Estatus = newStatus;

            try
            {
                bool updated = await _citasDbService.UpdateCitaStatusAsync(id, newStatus);

                if (!updated)
                {
                    return NotFound($"Cita con ID {id} no encontrada para actualizar el estatus.");
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en PUT /api/Citas/{Id}/estatus: {Message}", id, ex.Message);
                return StatusCode(500, "Error interno del servidor al actualizar el estatus.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            try
            {
                var existingCita = await _citasDbService.GetCitaByIdAsync(id);
                if (existingCita == null)
                {
                    return NotFound($"Cita con ID {id} no encontrada para eliminar.");
                }

                await _citasDbService.DeleteCitaAsync(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/Citas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
    }
}
