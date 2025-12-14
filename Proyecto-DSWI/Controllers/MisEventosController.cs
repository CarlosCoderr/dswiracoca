using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;

namespace Proyecto_DSWI.Controllers
{
    public class MisEventosController : Controller
    {
        private readonly MisEventosRepository _repo;

        public MisEventosController(MisEventosRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            
            int organizacionId = 1;

            var eventos = await _repo.ListarPorOrganizacionAsync(organizacionId);
            return View(eventos);
        }
    }
}
