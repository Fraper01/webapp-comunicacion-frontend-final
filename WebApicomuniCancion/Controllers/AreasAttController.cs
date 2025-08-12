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
        private readonly ILogger<AreasController> _logger;

        private readonly IAreasAttDbService _areasAttDbService; 

        public AreasAttController(IAreasAttDbService areasAttDbService, ILogger<AreasController> logger) 
        {
            _areasAttDbService = areasAttDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Areas_Att>>> GetAreasAtt()
        {
            try
            {
                var areas = await _areasAttDbService.GetAllAreasAttAsync();
                return Ok(areas); 
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error en GET /api/AreasAtt: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); 
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Areas_Att>> GetAreaAtt(int id)
        {
            try
            {
                var areaAtt = await _areasAttDbService.GetAreaAttByIdAsync(id);

                if (areaAtt == null) 
                {
                    return NotFound($"ÁreaAtt con ID {id} no encontrada."); 
                }

                return Ok(areaAtt); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/AreasAtt/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Area>> PostArea([FromBody] Areas_Att areaAtt)
        {
            if (string.IsNullOrWhiteSpace(areaAtt.Area_Desarrollo))
            {
                return BadRequest("El nombre del área es obligatorio."); 
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _areasAttDbService.AddAreaAttAsync(areaAtt);
                return Ok(areaAtt);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/AreasAtt: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(int id, [FromBody] Areas_Att areaAtt)
        {
            if (id != areaAtt.Id_AreaAtt)
            {
                return BadRequest("El ID de la URL no coincide con el ID del áreaAtt en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(areaAtt.Area_Desarrollo)) 
            {
                return BadRequest("El nombre del área es obligatorio para la actualización.");
            }

            try
            {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                var existingArea = await _areasAttDbService.GetAreaAttByIdAsync(id);
                if (existingArea == null)
                {
                    return NotFound($"ÁreaAtt con ID {id} no encontrada para eliminar.");
                }

                await _areasAttDbService.DeleteAreaAttAsync(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/AreasAtt/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
    }
}