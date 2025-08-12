using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppcomuniCanción.Models;
using System.Security.Claims;

namespace WebAppcomuniCanción.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            ViewBag.ShowAdminMenu = HttpContext.Session.GetString("IsUserAuthenticated") == "true";
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terapeutas()
        {
            return View();
        }
        public IActionResult LogopediaInfantil()
        {
            return View();
        }
        public IActionResult Contacta()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
