using Microsoft.AspNetCore.Mvc;
using WebAppcomuniCancion.Models; 
using WebAppcomuniCancion.Services;
using System.Threading.Tasks;
using WebAppcomuniCancion.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebAppcomuniCancion.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuariosApiService _usuariosApiService; 

        public LoginController(IUsuariosApiService usuariosApiService)
        {
            _usuariosApiService = usuariosApiService;
        }

        public IActionResult Index()
        {
            ViewBag.LoginMessage = null;
            ViewBag.LoginSuccess = false;
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(UsuariosLoginDto model)
        {
            ViewBag.LoginMessage = null;
            ViewBag.LoginSuccess = false;

            if (ModelState.IsValid)
            {
                bool isAuthenticated = await _usuariosApiService.AuthenticateUserAsync(model.User, model.Password);

                if (isAuthenticated)
                {
                    HttpContext.Session.SetString("IsUserAuthenticated", "true");
                    TempData["ShowLoginSuccessModal"] = true; 
                    model.Password = string.Empty;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.LoginMessage = "Usuario o contraseña incorrectos. Por favor, inténtalo de nuevo.";
                    ViewBag.LoginSuccess = false; 
                    model.Password = string.Empty;
                    return View("Index", model); 
                }
            }

            ViewBag.LoginMessage = "Por favor, completa todos los campos.";
            ViewBag.LoginSuccess = false; 
            return View("Index", model); 

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsUserAuthenticated"); 
            return RedirectToAction("Index", "Home"); 
        }
    }
}