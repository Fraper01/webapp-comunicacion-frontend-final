// Ubicación: WebApicomuniCancion/Controllers/AreasController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApicomuniCancion.Models.Entities;
using WebApicomuniCancion.Services;

namespace WebApicomuniCancion.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class AreasController : ControllerBase
    {
        // Si usaras ILogger para logging profesional:
        private readonly ILogger<AreasController> _logger;

        private readonly IAreasDbService _areasDbService; // Correcto: inyecta la interfaz

        // Constructor para inyección de dependencias
        public AreasController(IAreasDbService areasDbService, ILogger<AreasController> logger) 
        {
            _areasDbService = areasDbService;
            _logger = logger;
        }

        // GET: api/Areas - Obtener todas las áreas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            try
            {
                var areas = await _areasDbService.GetAllAreasAsync();
                return Ok(areas); // 200 OK con la lista de áreas
            }
            catch (Exception ex) // Captura cualquier error del servicio
            {
                // Registra el error (en producción usarías _logger.LogError)
                Console.Error.WriteLine($"Error en GET /api/Areas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); // 500 Internal Server Error
            }
        }

        // GET: api/Areas/5 - Obtener un área por ID
        [HttpGet("{id}")] 
        public async Task<ActionResult<Area>> GetArea(int id)
        {
            try
            {
                var area = await _areasDbService.GetAreaByIdAsync(id);

                if (area == null) // Si el servicio devuelve null, el ID no existe
                {
                    return NotFound($"Área con ID {id} no encontrada."); // 404 Not Found
                }

                return Ok(area); // 200 OK con el área encontrada
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/Areas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        // POST: api/Areas - Añadir una nueva área
        // El cuerpo de la solicitud (JSON) debe contener los datos del Área
        [HttpPost]
        public async Task<ActionResult<Area>> PostArea([FromBody] Area area)
        {
            // Validación de entrada simple en el controlador
            if (string.IsNullOrWhiteSpace(area.Area_Desarrollo))
            {
                return BadRequest("El nombre del área es obligatorio."); // 400 Bad Request
            }
            if (!ModelState.IsValid) 
            { 
                return BadRequest(ModelState); 
            }

            try
            {
                await _areasDbService.AddAreaAsync(area);
                // Si no obtuviste el ID autogenerado, puedes retornar StatusCode(201, area);
                //return CreatedAtAction(nameof(GetArea), new { id = area.ID }, area);
                return Ok(area);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/Areas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        // PUT: api/Areas/5 - Actualizar un área existente
        // El ID de la URL debe coincidir con el ID del objeto en el cuerpo
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(int id, [FromBody] Area area)
        {
            if (id != area.Id_Area)
            {
                return BadRequest("El ID de la URL no coincide con el ID del área en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(area.Area_Desarrollo)) // Validación de entrada
            {
                return BadRequest("El nombre del área es obligatorio para la actualización.");
            }

            try
            {
                // Verificar existencia antes de intentar actualizar (reutilizando GetAreaByIdAsync)
                var existingArea = await _areasDbService.GetAreaByIdAsync(id);
                if (existingArea == null)
                {
                    return NotFound($"Área con ID {id} no encontrada para actualizar.");
                }

                await _areasDbService.UpdateAreaAsync(area);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en PUT /api/Areas/{id}: {ex.Message}");
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
                var existingArea = await _areasDbService.GetAreaByIdAsync(id);
                if (existingArea == null)
                {
                    return NotFound($"Área con ID {id} no encontrada para eliminar.");
                }

                await _areasDbService.DeleteAreaAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/Areas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
    }
}