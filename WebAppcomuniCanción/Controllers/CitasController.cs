using Microsoft.AspNetCore.Mvc;
using WebAppcomuniCanción.Models;
using WebAppcomuniCanción.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; 
using System; 

namespace WebAppcomuniCanción.Controllers
{
    public class CitasController : Controller
    {
        private readonly ICitasApiService _citasApiService;
        private readonly ILogger<CitasController> logger;
        public CitasController(ICitasApiService citasApiService, ILogger<CitasController> _logger) // Inyección de dependencia
        {
            _citasApiService = citasApiService;
            logger = _logger;
        }

        public async Task<IActionResult> Index()
        {
            List<CitaDto>? citas = await _citasApiService.GetCitasAsync();

            if (citas == null)
            {
                // Puedes añadir un mensaje de error a ViewData o TempData si la API falla
                ViewBag.ErrorMessage = "No se pudieron cargar las citas. Intente de nuevo más tarde.";
                logger.LogError("GetCitasAsync devolvió null o falló.");
                citas = new List<CitaDto>(); // Para evitar errores en la vista si la lista es null
            }
            return View(citas);
        }
     
        // GET: Muestra el formulario para agendar una nueva cita
        public IActionResult Contacta()
        {
            // Pasa un nuevo CitaDto a la vista para que el formulario se enlace a él
            return View(new CitaDto());
        }

        // POST: Recibe los datos del formulario y los envía a la API
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección contra ataques CSRF
        public async Task<IActionResult> Contacta(CitaDto citaDto)
        {
            // Validar manualmente si al menos un turno está seleccionado
            if (!citaDto.Turno_Manana && !citaDto.Turno_Tarde && !citaDto.Turno_Noche && !citaDto.Turno_Sabado)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar al menos un turno (Mañana, Tarde, Noche o Sábado).");
            }
            // VALIDACIÓN MANUAL PARA EL CHECKBOX DE PRIVACIDAD
            bool privacidadAceptada = Request.Form.ContainsKey("privacidad");

            if (!privacidadAceptada)
            {
                ModelState.AddModelError(string.Empty, "Debe Dar su consentimiento Antes de Enviar este Formulario.");
            }
            if (ModelState.IsValid)
            {

                bool success = await _citasApiService.CreateCitaAsync(citaDto);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cita agendada exitosamente.";
                    return RedirectToAction("Confirmacion"); 
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al agendar la cita. Por favor, intente de nuevo.");
                }
            }

            return View(citaDto);
        }

        public IActionResult Confirmacion()
        {
            return View();
        }

        // POST: /CitasWeb/UpdateStatus (esto será llamado por JavaScript/AJAX desde la vista)
        // Esta acción actualizará el estatus de una cita específica.
        [HttpPost]
        [ValidateAntiForgeryToken] // Descomenta esta línea si estás usando tokens Anti-forgery
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewStatus))
            {
                return BadRequest("El nuevo estatus no puede estar vacío.");
            }

            // Opcional: Validar que newStatus sea uno de los valores permitidos en el cliente también
            string[] allowedStatuses = { "Pendiente", "Confirmada", "Cancelada", "Completada" };
            if (!allowedStatuses.Contains(request.NewStatus))
            {
                return BadRequest($"El estatus '{request.NewStatus}' no es válido.");
            }

            try
            {
                bool success = await _citasApiService.UpdateCitaStatusAsync(request.Id, request.NewStatus);

                if (success)
                {
                    return Json(new { success = true, message = "Estatus actualizado correctamente." });
                }
                else
                {
                    // Esto podría ser un 404 si la cita no se encontró en la API
                    return StatusCode(404, new { success = false, message = $"Cita con ID {request.Id} no encontrada o error al actualizar." });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar estatus de cita {CitaId}: {Message}", request.Id, ex.Message);
                return StatusCode(500, new { success = false, message = $"Error interno del servidor al actualizar el estatus: {ex.Message}" });
            }
        }


        // Modelo auxiliar para la solicitud de actualización de estatus
        public class UpdateStatusRequest
        {
            public int Id { get; set; }
            public string? NewStatus { get; set; }
        }

        // GET: /CitasWeb/Details/5 (para ver el detalle de una cita, si lo implementas)
        // Esta acción cargará los detalles de una cita específica.
        public async Task<IActionResult> Details(int id)
        {
            CitaDto? cita = await _citasApiService.GetCitaByIdAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            return View(cita);
        }

    }
}

