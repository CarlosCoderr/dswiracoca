using Microsoft.AspNetCore.Mvc;
using Racoca_DSWI.Models;

namespace Racoca_DSWI.Controllers
{
    public class VoluntarioController : Controller
    {
        [HttpGet]
        public IActionResult Paso1()
        {
            return View(new Voluntario());
        }

        [HttpPost]
        public IActionResult Paso1(Voluntario v)
        {
            
            return View("Paso2", v);
        }

        [HttpPost]
        public IActionResult Paso2(Voluntario v)
        {
            
            return View("Paso3", v);
        }

        [HttpPost]
        public IActionResult Paso3(Voluntario v)
        {
            
            return RedirectToAction("TipoUsuario", "Registro");
        }

        [HttpPost]
        public IActionResult VolverPaso1(Voluntario v) => View("Paso1", v);
        [HttpPost]
        public IActionResult VolverPaso2(Voluntario v) => View("Paso2", v);
    }
}