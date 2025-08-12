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
        public CitasController(ICitasApiService citasApiService, ILogger<CitasController> _logger) 
        {
            _citasApiService = citasApiService;
            logger = _logger;
        }

        public async Task<IActionResult> Index()
        {
            List<CitaDto>? citas = await _citasApiService.GetCitasAsync();

            if (citas == null)
            {
                ViewBag.ErrorMessage = "No se pudieron cargar las citas. Intente de nuevo más tarde.";
                logger.LogError("GetCitasAsync devolvió null o falló.");
                citas = new List<CitaDto>(); 
            }
            return View(citas);
        }
     
        public IActionResult Contacta()
        {
            return View(new CitaDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Contacta(CitaDto citaDto)
        {
            if (!citaDto.Turno_Manana && !citaDto.Turno_Tarde && !citaDto.Turno_Noche && !citaDto.Turno_Sabado)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar al menos un turno (Mañana, Tarde, Noche o Sábado).");
            }
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

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewStatus))
            {
                return BadRequest("El nuevo estatus no puede estar vacío.");
            }

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
                    return StatusCode(404, new { success = false, message = $"Cita con ID {request.Id} no encontrada o error al actualizar." });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar estatus de cita {CitaId}: {Message}", request.Id, ex.Message);
                return StatusCode(500, new { success = false, message = $"Error interno del servidor al actualizar el estatus: {ex.Message}" });
            }
        }


        public class UpdateStatusRequest
        {
            public int Id { get; set; }
            public string? NewStatus { get; set; }
        }

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

