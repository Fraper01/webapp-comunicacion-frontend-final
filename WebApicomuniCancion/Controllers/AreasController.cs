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
        private readonly ILogger<AreasController> _logger;

        private readonly IAreasDbService _areasDbService; 

        public AreasController(IAreasDbService areasDbService, ILogger<AreasController> logger) 
        {
            _areasDbService = areasDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            try
            {
                var areas = await _areasDbService.GetAllAreasAsync();
                return Ok(areas); 
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error en GET /api/Areas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); 
            }
        }

        [HttpGet("{id}")] 
        public async Task<ActionResult<Area>> GetArea(int id)
        {
            try
            {
                var area = await _areasDbService.GetAreaByIdAsync(id);

                if (area == null) 
                {
                    return NotFound($"Área con ID {id} no encontrada."); 
                }

                return Ok(area); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/Areas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Area>> PostArea([FromBody] Area area)
        {
            if (string.IsNullOrWhiteSpace(area.Area_Desarrollo))
            {
                return BadRequest("El nombre del área es obligatorio."); 
            }
            if (!ModelState.IsValid) 
            { 
                return BadRequest(ModelState); 
            }

            try
            {
                await _areasDbService.AddAreaAsync(area);
                return Ok(area);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/Areas: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(int id, [FromBody] Area area)
        {
            if (id != area.Id_Area)
            {
                return BadRequest("El ID de la URL no coincide con el ID del área en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(area.Area_Desarrollo)) 
            {
                return BadRequest("El nombre del área es obligatorio para la actualización.");
            }

            try
            {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                var existingArea = await _areasDbService.GetAreaByIdAsync(id);
                if (existingArea == null)
                {
                    return NotFound($"Área con ID {id} no encontrada para eliminar.");
                }

                await _areasDbService.DeleteAreaAsync(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/Areas/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
    }
}