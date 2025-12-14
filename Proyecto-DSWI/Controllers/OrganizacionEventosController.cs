using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Proyecto_DSWI.Data;
using Proyecto_DSWI.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_DSWI.Controllers
{
    public class OrganizacionEventosController : Controller
    {
        private readonly CrearEventoRepository _crearRepo;
        private readonly CategoriaEventoRepository _categoriaRepo;
        private readonly DistritoRepository _distritoRepo;
        private readonly IWebHostEnvironment _env;

        public OrganizacionEventosController(
            CrearEventoRepository crearRepo,
            CategoriaEventoRepository categoriaRepo,
            DistritoRepository distritoRepo,
            IWebHostEnvironment env)
        {
            _crearRepo = crearRepo;
            _categoriaRepo = categoriaRepo;
            _distritoRepo = distritoRepo;
            _env = env;
        }

        private async Task CargarCombosAsync()
        {
            ViewBag.Categorias = await _categoriaRepo.ListarAsync();
            ViewBag.Distritos = await _distritoRepo.ListarActivosAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarCombosAsync();
            return View(new CrearEventoModel
            {
                FechaEvento = DateTime.Today,
                Ilimitado = true,
                Estado = "Activo"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearEventoModel model)
        {
            await CargarCombosAsync();

            // ✅ fecha >= hoy
            if (model.FechaEvento.Date < DateTime.Today)
                ModelState.AddModelError(nameof(model.FechaEvento), "La fecha debe ser hoy o una fecha futura.");

            // ✅ galería max 15
            if (model.GaleriaFiles != null && model.GaleriaFiles.Count > 15)
                ModelState.AddModelError(nameof(model.GaleriaFiles), "Máximo 15 imágenes en la galería.");

            if (!ModelState.IsValid)
                return View(model);

            // ⚠️ aquí saca tu organizacionId real (session/auth). Pongo ejemplo fijo:
            int organizacionId = 1;

            // 1) Crear evento (sin imagen_principal todavía)
            int eventoId = await _crearRepo.CrearEventoAsync(organizacionId, model);

            // 2) Crear carpeta
            var folder = Path.Combine(_env.WebRootPath, "uploads", "eventos", eventoId.ToString());
            Directory.CreateDirectory(folder);

            // 3) Guardar imagen principal (si vino)
            if (model.ImagenPrincipalFile != null && model.ImagenPrincipalFile.Length > 0)
            {
                var ext = Path.GetExtension(model.ImagenPrincipalFile.FileName);
                var fileName = $"principal_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(folder, fileName);

                await using (var fs = new FileStream(filePath, FileMode.Create))
                    await model.ImagenPrincipalFile.CopyToAsync(fs);

                var url = $"/uploads/eventos/{eventoId}/{fileName}";
                await _crearRepo.ActualizarImagenPrincipalAsync(eventoId, url);
            }

            // 4) Guardar galería
            if (model.GaleriaFiles != null && model.GaleriaFiles.Any())
            {
                foreach (var f in model.GaleriaFiles.Where(x => x != null && x.Length > 0))
                {
                    var ext = Path.GetExtension(f.FileName);
                    var fileName = $"gal_{Guid.NewGuid():N}{ext}";
                    var filePath = Path.Combine(folder, fileName);

                    await using (var fs = new FileStream(filePath, FileMode.Create))
                        await f.CopyToAsync(fs);

                    var url = $"/uploads/eventos/{eventoId}/{fileName}";
                    await _crearRepo.InsertarImagenGaleriaAsync(eventoId, url);
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
