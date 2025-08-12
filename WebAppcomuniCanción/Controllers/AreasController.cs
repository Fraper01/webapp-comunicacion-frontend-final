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


        public AreasController(IAreasApiService areasApiService, ILogger<AreasController> _logger) // Inyección de dependencia
        {
            _areasApiService = areasApiService;
            logger = _logger;
        }
        public async Task<IActionResult> Index() 
        {
            List<AreaDto> areas = await _areasApiService.GetAreasAsync();
            return View(areas); // Pasa la lista de áreas a la vista
        }
        [HttpGet]
        public async Task<IActionResult> AgregarArea()
        {
            try
            {
                // Obtener la lista de especialidades desde la API solo para que dispare la excepción si la webapi no está corriendo
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
                        logger.LogError(ex, "Error al obtener Areas de Desarrollo: Conexión rechazada."); // Log
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
                    // Llamada a la API
                    bool agregado = await _areasApiService.CreateAreaAsync(areadto);
                    if (agregado)
                    {
                        TempData["MensajeExito"] = "Area de Desarrollo agregada correctamente.";
                        return RedirectToAction(nameof(Index)); // Redirigir a la lista de areas de desarrollo
                    }
                    else
                    {
                        TempData["MensajeError"] = "Error al agregar el area de desarrollo.";
                        ModelState.AddModelError(string.Empty, "Error al agregar el Area de Desarrollo.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error
                    TempData["MensajeError"] = ex.Message;
                    ModelState.AddModelError(string.Empty, "Error al agregar el Area de Desarrollo.");
                }
            }
            return View(areadto); // Mostrar la vista con errores
        }
        // GET: Areas/Edit/5
        // Muestra el formulario de edición con los datos del área precargados.
        public async Task<IActionResult> EditarArea(int? id) 
        {
            if (id == null)
            {
                return NotFound(); // Si no se proporciona un ID, es un error 404
            }

            var area = await _areasApiService.GetAreaByIdAsync(id.Value); 

            if (area == null)
            {
                return NotFound(); // Si el área no se encuentra en la API
            }

            return View(area); // Pasa el AreaDto a la vista
        }

        // Procesa los datos enviados desde el formulario de edición para actualizar el área.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarArea(int id, AreaDto areaDto)
        {
            // Validar que el ID de la URL coincida con el ID del objeto enviado
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
                    return RedirectToAction(nameof(Index)); // Redirige a la lista después de actualizar
                }
                ModelState.AddModelError("", "Error al actualizar el área de desarrollo en la API.");
            }
            return View(areaDto);
        }
    }
}
