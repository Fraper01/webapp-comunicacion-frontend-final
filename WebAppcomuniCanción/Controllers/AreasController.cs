using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppcomuniCanción.Models;
using WebAppcomuniCanción.Services;

namespace WebAppcomuniCanción.Controllers
{
    public class AreasController : Controller
    {
        private readonly IAreasApiService _areasApiService;
        private readonly ILogger<AreasController> logger; 


        public AreasController(IAreasApiService areasApiService, ILogger<AreasController> _logger) 
        {
            _areasApiService = areasApiService;
            logger = _logger;
        }
        public async Task<IActionResult> Index() 
        {
            List<AreaDto> areas = await _areasApiService.GetAreasAsync();
            return View(areas); 
        }
        [HttpGet]
        public async Task<IActionResult> AgregarArea()
        {
            try
            {
                List<AreaDto> especialidadesDto = await _areasApiService.GetAreasAsync();
                ViewBag.titulo = "AGREGAR UNA NUEVA AREA DE DESARROLLO";
                return View();
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException is System.Net.Sockets.SocketException socketEx)
                {
                    if (socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionRefused)
                    {
                        TempData["MensajeError"] = "No se pudo conectar con la API de Areas de Desarrollo. Compruebe que la API esté en ejecución.";
                        logger.LogError(ex, "Error al obtener Areas de Desarrollo: Conexión rechazada."); 
                    }
                    else
                    {
                        TempData["MensajeError"] = "Error de red al obtener la lista de Areas de Desarrollo.";
                        logger.LogError(ex, "Error al obtener Areas de Desarrollo: Error de red.");
                    }
                }
                else
                {
                    TempData["MensajeError"] = "Error de HTTP al obtener la lista de Areas de Desarrollo.";
                    logger.LogError(ex, "Error al obtener Areas de Desarrollo: Error de HTTP.");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error inesperado al obtener la lista de Areas de Desarrollo.";
                logger.LogError(ex, "Error inesperado al obtener Areas de Desarrollo.");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarArea(AreaDto areadto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool agregado = await _areasApiService.CreateAreaAsync(areadto);
                    if (agregado)
                    {
                        TempData["MensajeExito"] = "Area de Desarrollo agregada correctamente.";
                        return RedirectToAction(nameof(Index)); 
                    }
                    else
                    {
                        TempData["MensajeError"] = "Error al agregar el area de desarrollo.";
                        ModelState.AddModelError(string.Empty, "Error al agregar el Area de Desarrollo.");
                    }
                }
                catch (Exception ex)
                {
                    TempData["MensajeError"] = ex.Message;
                    ModelState.AddModelError(string.Empty, "Error al agregar el Area de Desarrollo.");
                }
            }
            return View(areadto); 
        }
        public async Task<IActionResult> EditarArea(int? id) 
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var area = await _areasApiService.GetAreaByIdAsync(id.Value); 

            if (area == null)
            {
                return NotFound(); 
            }

            return View(area); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarArea(int id, AreaDto areaDto)
        {
            if (id != areaDto.Id_Area)
            {
                return BadRequest("ID de área no válido.");
            }

            if (ModelState.IsValid)
            {
                bool success = await _areasApiService.UpdateAreaAsync(areaDto);
                if (success)
                {
                    TempData["SuccessMessage"] = "Área de Desarrollo actualizada exitosamente.";
                    return RedirectToAction(nameof(Index)); 
                }
                ModelState.AddModelError("", "Error al actualizar el área de desarrollo en la API.");
            }
            return View(areaDto);
        }
    }
}
