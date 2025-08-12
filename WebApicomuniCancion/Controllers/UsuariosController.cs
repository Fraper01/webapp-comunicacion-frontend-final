
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
        // Si usaras ILogger para logging profesional:
        private readonly ILogger<UsuariosController> _logger;

        private readonly IUsuariosDbService _usuariosDbService; // Correcto: inyecta la interfaz

        // Constructor para inyección de dependencias
        public UsuariosController(IUsuariosDbService usuariosDbService, ILogger<UsuariosController> logger) // <-- Inyectamos ILogger
        {
            _usuariosDbService = usuariosDbService;
            _logger = logger;
        }

        // GET: api/Usuario - Obtener todas las usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            try
            {
                var areas = await _usuariosDbService.GetAllUsuariosAsync();
                return Ok(areas); // 200 OK con la lista de áreas
            }
            catch (Exception ex) // Captura cualquier error del servicio
            {
                // Registra el error (en producción usarías _logger.LogError)
                Console.Error.WriteLine($"Error en GET /api/Usuarios: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar los datos."); // 500 Internal Server Error
            }
        }

        // GET: api/Usuarios/5 - Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            try
            {
                var area = await _usuariosDbService.GetUsuarioByIdAsync(id);

                if (area == null) // Si el servicio devuelve null, el ID no existe
                {
                    return NotFound($"Usuario con ID {id} no encontrada."); // 404 Not Found
                }

                return Ok(area); // 200 OK con el área encontrada
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GET /api/Usuarios/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al recuperar el dato.");
            }
        }

        // POST: api/Usuarios - Añadir un nuevo Usuario
        // El cuerpo de la solicitud (JSON) debe contener los datos del Usuario
        [HttpPost]
        public async Task<ActionResult<Usuarios>> PostArea([FromBody] Usuarios usuarios)
        {
            // Validación de entrada simple en el controlador
            if (string.IsNullOrWhiteSpace(usuarios.full_name))
            {
                return BadRequest("El nombre del usuario es obligatorio."); // 400 Bad Request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(usuarios.user))
            {
                return BadRequest("El codigo del usuario es obligatorio."); // 400 Bad Request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(usuarios.password))
            {
                return BadRequest("El password del usuario es obligatorio."); // 400 Bad Request
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

        // PUT: api/Usuarios/5 - Actualizar un usuario existente
        // El ID de la URL debe coincidir con el ID del objeto en el cuerpo
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] Usuarios usuarios)
        {
            if (id != usuarios.id_user)
            {
                return BadRequest("El ID de la URL no coincide con el ID del usuario en el cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(usuarios.full_name)) // Validación de entrada
            {
                return BadRequest("El nombre del usuario es obligatorio para la actualización.");
            }
            if (string.IsNullOrWhiteSpace(usuarios.user)) // Validación de entrada
            {
                return BadRequest("El codigo del usuario es obligatorio para la actualización.");
            }
            if (string.IsNullOrWhiteSpace(usuarios.password)) // Validación de entrada
            {
                return BadRequest("El password del usuario es obligatorio para la actualización.");
            }

            try
            {
                // Verificar existencia antes de intentar actualizar (reutilizando GetUsuarioByIdAsync)
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

        // DELETE: api/Usuarios/5 - Eliminar un usuarios
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                // Verificar existencia antes de intentar eliminar
                var existingUsuario = await _usuariosDbService.GetUsuarioByIdAsync(id);
                if (existingUsuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrada para eliminar.");
                }

                await _usuariosDbService.DeleteUsuarioAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en DELETE /api/Usuarios/{id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar el dato.");
            }
        }
        // POST: api/Usuarios/Authenticate
        [HttpPost("Authenticate")] 
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            // Validar que los datos de entrada no estén vacíos
            if (string.IsNullOrEmpty(request.User) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Se requiere usuario y contraseña." });
            }

            bool isValidUser = await _usuariosDbService.ValidateCredentialsAsync(request.User, request.Password);

            if (isValidUser)
            {
                // Usuario autorizado: Puedes devolver un mensaje de éxito.
                return Ok(new { message = "Autenticación exitosa." });
            }
            else
            {
                // Usuario no autorizado
                return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
            }
        }

    }
}
