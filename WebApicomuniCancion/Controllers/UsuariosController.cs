
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApicomuniCancion.Interfaces;
using WebApicomuniCancion.Models.Entities;
using WebApicomuniCancion.Models.DTOs;
using WebApicomuniCancion.Services;

namespace WebApicomuniCancion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ILogger<UsuariosController> _logger;

        private readonly IUsuariosDbService _usuariosDbService; 

        public UsuariosController(IUsuariosDbService usuariosDbService, ILogger<UsuariosController> logger) 
        {
            _usuariosDbService = usuariosDbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            try
            {
                var areas = await _usuariosDbService.GetAllUsuariosAsync();
                return Ok(areas); 
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Error en GET /api/Usuarios: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); 
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            try
            {
                var area = await _usuariosDbService.GetUsuarioByIdAsync(id);

                if (area == null) 
                {
                    return NotFound($"Usuario con ID {id} no encontrada."); 
                }

                return Ok(area); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/Usuarios/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Usuarios>> PostArea([FromBody] Usuarios usuarios)
        {
            if (string.IsNullOrWhiteSpace(usuarios.full_name))
            {
                return BadRequest("El nombre del usuario es obligatorio."); 
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(usuarios.user))
            {
                return BadRequest("El codigo del usuario es obligatorio."); 
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(usuarios.password))
            {
                return BadRequest("El password del usuario es obligatorio.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _usuariosDbService.AddUsuariosAsync(usuarios);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en POST /api/Usuarios: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al añadir el dato.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] Usuarios usuarios)
        {
            if (id != usuarios.id_user)
            {
                return BadRequest("El ID de la URL no coincide con el ID del usuario en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(usuarios.full_name)) 
            {
                return BadRequest("El nombre del usuario es obligatorio para la actualización.");
            }
            if (string.IsNullOrWhiteSpace(usuarios.user)) 
            {
                return BadRequest("El codigo del usuario es obligatorio para la actualización.");
            }
            if (string.IsNullOrWhiteSpace(usuarios.password)) 
            {
                return BadRequest("El password del usuario es obligatorio para la actualización.");
            }

            try
            {
                var existingUsuario = await _usuariosDbService.GetUsuarioByIdAsync(id);
                if (existingUsuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrada para actualizar.");
                }

                await _usuariosDbService.UpdateUsuariosAsync(usuarios);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en PUT /api/Usuarios/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al actualizar el dato.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var existingUsuario = await _usuariosDbService.GetUsuarioByIdAsync(id);
                if (existingUsuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrada para eliminar.");
                }

                await _usuariosDbService.DeleteUsuarioAsync(id);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/Usuarios/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
        [HttpPost("Authenticate")] 
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.User) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Se requiere usuario y contraseña." });
            }

            bool isValidUser = await _usuariosDbService.ValidateCredentialsAsync(request.User, request.Password);

            if (isValidUser)
            {
                return Ok(new { message = "Autenticación exitosa." });
            }
            else
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }
        }

    }
}
