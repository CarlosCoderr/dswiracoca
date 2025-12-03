using Microsoft.AspNetCore.Mvc;
using Racoca_DSWI.Models;

namespace Racoca_DSWI.Controllers
{
    public class OrganizacionController : Controller
    {
        [HttpGet]
        public IActionResult Paso1()
        {
            return View(new Organizacion());
        }

        [HttpPost]
        public IActionResult Paso1(Organizacion o)
        {
            if (!ModelState.IsValid)
                return View("Paso1", o);

            return View("Paso2", o);
        }

        [HttpPost]
        public IActionResult Paso2(Organizacion o)
        {
            if (!ModelState.IsValid)
                return View("Paso2", o);

            return View("Paso3", o);
        }

        [HttpPost]
        public IActionResult Paso3(Organizacion o)
        {
            if (!ModelState.IsValid)
                return View("Paso3", o);

            return View("Paso4", o);
        }

        [HttpPost]
        public IActionResult Paso4(Organizacion o)
        {
            if (!o.AceptoTerminos || !o.AutorizaDatos)
            {
                ModelState.AddModelError("", "Debes aceptar los términos y la autorización.");
                return View("Paso4", o);
            }

            return RedirectToAction("TipoUsuario", "Registro");

        }

        [HttpPost]
        public IActionResult VolverPaso1(Organizacion o)
        {
            return View("Paso1", o);
        }

        [HttpPost]
        public IActionResult VolverPaso2(Organizacion o)
        {
            return View("Paso2", o);
        }

        [HttpPost]
        public IActionResult VolverPaso3(Organizacion o)
        {
            return View("Paso3", o);
        }
    }
}
