// Ubicación: WebApicomuniCancion/Controllers/AreasController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApicomuniCancion.Interfaces;
using WebApicomuniCancion.Models.Entities;
using WebApicomuniCancion.Services;

namespace WebApicomuniCancion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasAttController : ControllerBase
    {
        // Si usaras ILogger para logging profesional:
        private readonly ILogger<AreasController> _logger;

        private readonly IAreasAttDbService _areasAttDbService; // Correcto: inyecta la interfaz

        // Constructor para inyección de dependencias
        public AreasAttController(IAreasAttDbService areasAttDbService, ILogger<AreasController> logger) // <-- Inyectamos ILogger
        {
            _areasAttDbService = areasAttDbService;
            _logger = logger;
        }

        // GET: api/Areas - Obtener todas las áreas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Areas_Att>>> GetAreasAtt()
        {
            try
            {
                var areas = await _areasAttDbService.GetAllAreasAttAsync();
                return Ok(areas); // 200 OK con la lista de áreas
            }
            catch (Exception ex) // Captura cualquier error del servicio
            {
                // Registra el error (en producción usarías _logger.LogError)
                Console.Error.WriteLine($"Error en GET /api/AreasAtt: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); // 500 Internal Server Error
            }
        }

        // GET: api/Areas/5 - Obtener un área por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Areas_Att>> GetAreaAtt(int id)
        {
            try
            {
                var areaAtt = await _areasAttDbService.GetAreaAttByIdAsync(id);

                if (areaAtt == null) // Si el servicio devuelve null, el ID no existe
                {
                    return NotFound($"ÁreaAtt con ID {id} no encontrada."); // 404 Not Found
                }

                return Ok(areaAtt); // 200 OK con el área encontrada
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/AreasAtt/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        // POST: api/AreasAtt - Añadir una nueva áreaAtt
        [HttpPost]
        public async Task<ActionResult<Area>> PostArea([FromBody] Areas_Att areaAtt)
        {
            // Validación de entrada simple en el controlador
            if (string.IsNullOrWhiteSpace(areaAtt.Area_Desarrollo))
            {
                return BadRequest("El nombre del área es obligatorio."); // 400 Bad Request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _areasAttDbService.AddAreaAttAsync(areaAtt);
                // Si no obtuviste el ID autogenerado, puedes retornar StatusCode(201, areaAtt);
                return Ok(areaAtt);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/AreasAtt: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        // PUT: api/Areas/5 - Actualizar un área existente
        // El ID de la URL debe coincidir con el ID del objeto en el cuerpo
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(int id, [FromBody] Areas_Att areaAtt)
        {
            if (id != areaAtt.Id_AreaAtt)
            {
                return BadRequest("El ID de la URL no coincide con el ID del áreaAtt en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(areaAtt.Area_Desarrollo)) // Validación de entrada
            {
                return BadRequest("El nombre del área es obligatorio para la actualización.");
            }

            try
            {
                // Verificar existencia antes de intentar actualizar (reutilizando GetAreaByIdAsync)
                var existingArea = await _areasAttDbService.GetAreaAttByIdAsync(id);
                if (existingArea == null)
                {
                    return NotFound($"ÁreaAtt con ID {id} no encontrada para actualizar.");
                }

                await _areasAttDbService.UpdateAreaAttAsync(areaAtt);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en PUT /api/AreasAtt/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al actualizar el dato.");
            }
        }

        // DELETE: api/Areas/5 - Eliminar un área
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                // Verificar existencia antes de intentar eliminar
                var existingArea = await _areasAttDbService.GetAreaAttByIdAsync(id);
                if (existingArea == null)
                {
                    return NotFound($"ÁreaAtt con ID {id} no encontrada para eliminar.");
                }

                await _areasAttDbService.DeleteAreaAttAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/AreasAtt/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
    }
}