using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventoRepository _repo;
        private readonly EventoDetalleRepository _repoDetalle;


        public EventosController(EventoRepository repo, EventoDetalleRepository repoDetalle)
        {
            _repo = repo; 
        _repoDetalle = repoDetalle;
        }

        //public async Task<IActionResult> Detalle(int id)
        //{
        //    var evento = await _repo.ObtenerAsync(id);
        //    if (evento == null)
        //        return NotFound();

        //    return View(evento);
        //}

        public async Task<IActionResult> Index(string? q, string? distrito, int? categoriaId, DateTime? fecha, int page = 1)
        {
            int pageSize = 6;
            var paged = await _repo.ListarPaginadoAsync(q, distrito, categoriaId, fecha, page, pageSize);

            var vm = new EventosFiltroVM
            {
                Eventos = paged.Items,
                CategoriaId = categoriaId,
                Fecha = fecha,
                Q = q,
                Distrito = distrito,
                Page = paged.Page,
                TotalPages = paged.TotalPages
            };

           

            return View(vm);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var evento = await _repoDetalle.ObtenerAsync(id);
            if (evento == null) return NotFound();
            return View(evento);
        }
    }
}

