using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DistritoRepository _distritoRepo;
        private readonly CategoriaEventoRepository _categoriaRepo;
        private readonly EventoRepository _eventoRepo;

        public HomeController(
            DistritoRepository distritoRepo,
            CategoriaEventoRepository categoriaRepo,
            EventoRepository eventoRepo
        )
        {
            _distritoRepo = distritoRepo;
            _categoriaRepo = categoriaRepo;
            _eventoRepo = eventoRepo;
        }

        private async Task CargarCombosAsync()
        {
            ViewBag.Distritos = await _distritoRepo.ListarActivosAsync(); 
            ViewBag.Categorias = await _categoriaRepo.ListarAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, string? distrito, int? categoriaId, DateTime? fecha)
        {
            await CargarCombosAsync();

            var eventos = await _eventoRepo.ListarAsync(q, distrito, categoriaId, fecha);

            var vm = new EventosFiltroVM
            {
                Q = q,
                Distrito = distrito,
                CategoriaId = categoriaId,
                Fecha = fecha,
                Eventos = eventos
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Contacto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contacto(string nombre, string apellidos, string correo, string celular, string asunto, string mensaje)
        {
            TempData["ContactoOK"] = "Gracias por escribirnos. Nos pondremos en contacto pronto.";
            return RedirectToAction(nameof(Contacto));
        }
    }
}
