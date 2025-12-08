using Microsoft.AspNetCore.Mvc;
using Racoca_DSWI.Models;
using Racoca_DSWI.Data;
using System.Linq;

namespace Racoca_DSWI.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventoRepository _repo;

        public EventosController(EventoRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index(string buscar, string categoria, string ubicacion, int pagina = 1)
        {
            var eventos = _repo.ListarEventos()
            .SelectMany(e => e.Categoria.Split(','), (evento, cat) => new Evento
            {
                Id = evento.Id,
                Titulo = evento.Titulo,
                Descripcion = evento.Descripcion,
                Imagen = evento.Imagen,
                Ubicacion = evento.Ubicacion,
                Organizacion = evento.Organizacion,
                Cupos = evento.Cupos,

                Categoria = cat.Trim()
            })
            .ToList();


            if (!string.IsNullOrEmpty(buscar))
            {
                eventos = eventos
                    .Where(e => e.Titulo.ToLower().Contains(buscar.ToLower()))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                eventos = eventos
                    .Where(e => e.Categoria == categoria)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(ubicacion))
            {
                eventos = eventos
                    .Where(e => e.Ubicacion == ubicacion)
                    .ToList();
            }

            int tamañoPagina = 8;
            int totalRegistros = eventos.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamañoPagina);

            eventos = eventos
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(eventos);
        }
    }
}
s