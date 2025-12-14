using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Controllers
{
    public class IniciarSesionController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo;
        private readonly PasswordHasher<string> _hasher = new();

        public IniciarSesionController(UsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var email = (vm.Email ?? "").Trim().ToLower();
            var user = await _usuarioRepo.ObtenerPorEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(vm);
            }

            if (!string.Equals(user.Estado, "ACTIVO"))
            {
                ModelState.AddModelError("", "Tu cuenta no está activa.");
                return View(vm);
            }

            var verify = _hasher.VerifyHashedPassword(email, user.PasswordHash, vm.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(vm);
            }


            var nombre = await _usuarioRepo.ObtenerNombreParaSaludoAsync(user.Id, user.Rol);
            nombre ??= user.Email;


            HttpContext.Session.SetInt32("USER_ID", user.Id);
            HttpContext.Session.SetString("USER_EMAIL", user.Email);
            HttpContext.Session.SetString("USER_ROL", user.Rol);
            HttpContext.Session.SetString("USER_NOMBRE", nombre);
            HttpContext.Session.SetString("USER_FOTO", user.FotoPerfilUrl ?? "");


            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
