using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Controllers
{
    public class PerfilController : Controller
    {
        private readonly PerfilRepository _repo;
        public PerfilController(PerfilRepository repo) => _repo = repo;

        // PERFIL  LECTURA
        public IActionResult Voluntario(int id)
        {
            var vm = _repo.ObtenerVoluntario(id);
            if (vm == null) return NotFound("No existe voluntario o conexión mal.");

            return View("VoluntarioPerfil", vm);
        }

        [HttpGet]
        public IActionResult EditarVoluntario(int id)
        {
            var vm = _repo.ObtenerVoluntarioParaEditar(id);
            if (vm == null) return NotFound("No se encontró el voluntario para editar.");

            return View(vm); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarVoluntario(VoluntarioPerfilEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _repo.ActualizarVoluntario(model);

            TempData["OK"] = $"Perfil de {model.Nombres} actualizado correctamente";

            return RedirectToAction("Voluntario", new { id = model.UsuarioId });
        }
    }
}
