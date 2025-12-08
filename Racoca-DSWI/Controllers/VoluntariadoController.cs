using Microsoft.AspNetCore.Mvc;

namespace Racoca_DSWI.Controllers
{
    public class VoluntariadoController : Controller
    {
        [HttpGet]
        public IActionResult Detalle()
        {
            // Renderiza la vista de detalle ubicada en Views/Voluntariado/Detalle.cshtml
            return View();
        }

        [HttpPost]
        public IActionResult Inscribirse([FromForm] DateTime? fecha, [FromForm] TimeSpan? desde, [FromForm] TimeSpan? hasta, [FromForm] int participantes = 1)
        {
            // TODO: Persistir inscripción si se requiere.
            TempData["InscripcionMensaje"] = "Inscripción registrada. Te enviaremos un correo con la confirmación.";

            // Regresar al detalle con mensaje
            return RedirectToAction(nameof(Detalle));
        }
    }
}
