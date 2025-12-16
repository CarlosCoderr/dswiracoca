using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;

namespace Proyecto_DSWI.Controllers
{
    public class PerfilController : Controller
    {
        private readonly PerfilRepository _repo;
        public PerfilController(PerfilRepository repo) => _repo = repo;

        public IActionResult Voluntario(int id)
        {
            var vm = _repo.ObtenerVoluntario(id);
            if (vm == null) return NotFound("No existe voluntario o conexión mal.");
            return View("VoluntarioPerfil", vm);
        }
    }
}
