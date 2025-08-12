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
        // Si usaras ILogger para logging profesional:
        private readonly ILogger<CitasController> _logger;

        private readonly ICitasDbService _citasDbService; // Correcto: inyecta la interfaz

        // Constructor para inyección de dependencias
        public CitasController(ICitasDbService citasDbService, ILogger<CitasController> logger)
        {
            _citasDbService = citasDbService;
            _logger = logger;
        }

        // GET: api/Citas - Obtener todas las citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            try
            {
                var citas = await _citasDbService.GetAllCitasAsync();
                return Ok(citas); // 200 OK con la lista de áreas
            }
            catch (Exception ex) // Captura cualquier error del servicio
            {
                // Registra el error (en producción usarías _logger.LogError)
                Console.Error.WriteLine($"Error en GET /api/Citas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); // 500 Internal Server Error
            }
        }

        // GET: api/Citas/5 - Obtener un área por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            try
            {
                var cita = await _citasDbService.GetCitaByIdAsync(id);

                if (cita == null) // Si el servicio devuelve null, el ID no existe
                {
                    return NotFound($"Área con ID {id} no encontrada."); // 404 Not Found
                }

                return Ok(cita); // 200 OK con el área encontrada
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/Citas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        // POST: api/Citas - Añadir una nueva cita
        // El cuerpo de la solicitud (JSON) debe contener los datos del Cita
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita([FromBody] Cita cita)
        {
            // Validación de entrada simple en el controlador
            if (string.IsNullOrWhiteSpace(cita.Nombre))
            {
                return BadRequest("El nombre de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Apellido))
            {
                return BadRequest("El apellido de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Email))
            {
                return BadRequest("El email de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Paciente))
            {
                return BadRequest("El tipo paciente de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Tratamiento))
            {
                return BadRequest("El tipo tratamiento de la cita es obligatorio."); // 400 Bad Request
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
                // Si no obtuviste el ID autogenerado, puedes retornar StatusCode(201, citas);
                //return CreatedAtAction(nameof(GetCita), new { id = cita.id_citas }, citas);
                return Ok(cita);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/Citas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        // PUT: api/Citas/5 - Actualizar un cita existente
        // El ID de la URL debe coincidir con el ID del objeto en el cuerpo
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, [FromBody] Cita cita)
        {
            if (id != cita.Id_Citas)
            {
                return BadRequest("El ID de la URL no coincide con el ID del citas en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(cita.Nombre)) // Validación de entrada
            {
                return BadRequest("El nombre del cita es obligatorio para la actualización.");
            }
            if (string.IsNullOrWhiteSpace(cita.Apellido))
            {
                return BadRequest("El apellido de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Email))
            {
                return BadRequest("El email de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Paciente))
            {
                return BadRequest("El tipo paciente de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrWhiteSpace(cita.Tipo_Tratamiento))
            {
                return BadRequest("El tipo tratamiento de la cita es obligatorio."); // 400 Bad Request
            }
            if (string.IsNullOrEmpty(cita.Estatus))
            {
                cita.Estatus = "Pendiente";
            }
            // Opcional: Validar que newStatus sea uno de los valores permitidos (ej. "Pendiente", "Confirmada", etc.)
            string[] allowedStatuses = { "Pendiente", "Confirmada", "Cancelada", "Completada" };
            if (!allowedStatuses.Contains(cita.Estatus))
            {
                return BadRequest($"El estatus '{cita.Estatus}' no es válido.");
            }

            try
            {
                // Verificar existencia antes de intentar actualizar (reutilizando GetAreaByIdAsync)
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

        [HttpPut("{id}/estatus")] // Ruta: api/Citas/{id}/estatus
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
            //_citasDbService.Entry(existingCita).State = EntityState.Modified; // Marca la entidad como modificada

            //try
            //{
            //    await _citasDbService.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    // Esto es para manejar si la entidad fue modificada por otro usuario al mismo tiempo
            //    if (!_context.Citas.Any(e => e.Id == id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw; // Re-lanza la excepción si no es por "no encontrado"
            //    }
            //}

            //return NoContent(); // 204 No Content - Indica que la operación fue exitosa pero no hay contenido para devolver

            // Reemplázalas con la llamada al nuevo método del servicio:
            try
            {
                // Llama al nuevo método del servicio para actualizar solo el estatus
                bool updated = await _citasDbService.UpdateCitaStatusAsync(id, newStatus);

                if (!updated)
                {
                    // Si updated es false, significa que no se encontró la cita con ese ID
                    return NotFound($"Cita con ID {id} no encontrada para actualizar el estatus.");
                }

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en PUT /api/Citas/{Id}/estatus: {Message}", id, ex.Message);
                return StatusCode(500, "Error interno del servidor al actualizar el estatus.");
            }
        }

        // DELETE: api/Citas/5 - Eliminar una cita por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            try
            {
                // Verificar existencia antes de intentar eliminar
                var existingCita = await _citasDbService.GetCitaByIdAsync(id);
                if (existingCita == null)
                {
                    return NotFound($"Cita con ID {id} no encontrada para eliminar.");
                }

                await _citasDbService.DeleteCitaAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/Citas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
    }
}
