using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;

namespace Proyecto_DSWI.Controllers
{
    public class MisInscripcionesController : Controller
    {
        private readonly MisInscripcionesRepository _repo;

        public MisInscripcionesController(MisInscripcionesRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {

            int voluntarioId = 2;

            var inscripciones = await _repo.ListarPorVoluntarioAsync(voluntarioId);
            return View(inscripciones);
        }
    }
}
