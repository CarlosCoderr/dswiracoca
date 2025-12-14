using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;

namespace Proyecto_DSWI.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventoDetalleRepository _repo;

        public EventosController(EventoDetalleRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var evento = await _repo.ObtenerAsync(id);
            if (evento == null)
                return NotFound();

            return View(evento);
        }
    }
}
