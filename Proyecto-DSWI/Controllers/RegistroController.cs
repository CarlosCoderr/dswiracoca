using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto_DSWI.Data;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Controllers
{
    public class RegistroController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo;
        private readonly VoluntarioRepository _volRepo;
        private readonly OrganizacionRepository _orgRepo;
        private readonly HabilidadRepository _habRepo;
        private readonly TipoOrganizacionRepository _tipoOrgRepo;
        private readonly DistritoRepository _distritoRepo;
        private readonly IWebHostEnvironment _env;

        private readonly PasswordHasher<string> _hasher = new();

        public RegistroController(
            UsuarioRepository usuarioRepo,
            VoluntarioRepository volRepo,
            OrganizacionRepository orgRepo,
            HabilidadRepository habRepo,
            TipoOrganizacionRepository tipoOrgRepo,
            DistritoRepository distritoRepo,
            IWebHostEnvironment env
        )
        {
            _usuarioRepo = usuarioRepo;
            _volRepo = volRepo;
            _orgRepo = orgRepo;
            _habRepo = habRepo;
            _tipoOrgRepo = tipoOrgRepo;
            _distritoRepo = distritoRepo;
            _env = env;
        }

        private void SetSession<T>(string key, T value)
            => HttpContext.Session.SetString(key, JsonSerializer.Serialize(value));

        private T? GetSession<T>(string key)
        {
            var raw = HttpContext.Session.GetString(key);
            return raw == null ? default : JsonSerializer.Deserialize<T>(raw);
        }

        private void RemoveSession(params string[] keys)
        {
            foreach (var k in keys)
                HttpContext.Session.Remove(k);
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VoluntarioPaso1()
        {
            var vm = GetSession<RegistroVoluntarioPaso1VM>("VOL_P1");
            return View(vm ?? new RegistroVoluntarioPaso1VM());
        }

        [HttpPost]
        public IActionResult VoluntarioPaso1(RegistroVoluntarioPaso1VM vm)
        {

            if (!vm.EsMayorEdad)
                ModelState.AddModelError(nameof(vm.EsMayorEdad), "Debes confirmar que eres mayor de edad.");

            if (!ModelState.IsValid)
                return View(vm);

            SetSession("VOL_P1", vm);
            return RedirectToAction(nameof(VoluntarioPaso2));
        }

        [HttpGet]
        public async Task<IActionResult> VoluntarioPaso2()
        {
            ViewBag.Habilidades = await _habRepo.ListarAsync();

            var vm = GetSession<RegistroVoluntarioPaso2VM>("VOL_P2");
            return View(vm ?? new RegistroVoluntarioPaso2VM());
        }

        [HttpPost]
        public async Task<IActionResult> VoluntarioPaso2(RegistroVoluntarioPaso2VM vm)
        {
            ViewBag.Habilidades = await _habRepo.ListarAsync();

            if (!ModelState.IsValid)
                return View(vm);

            SetSession("VOL_P2", vm);
            return RedirectToAction(nameof(VoluntarioPaso3));
        }

        [HttpGet]
        public IActionResult VoluntarioPaso3()
        {
         
            var saved = GetSession<RegistroVoluntarioPaso3Sesion>("VOL_P3");
            var vm = new RegistroVoluntarioPaso3VM();

            if (saved != null)
            {
                vm.Email = saved.Email ?? "";
                vm.AceptaTerminos = saved.AceptaTerminos;
                vm.AceptaPrivacidad = saved.AceptaPrivacidad;
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> VoluntarioPaso3(RegistroVoluntarioPaso3VM vm)
        {
          
            if (!vm.AceptaTerminos)
                ModelState.AddModelError(nameof(vm.AceptaTerminos), "Debes aceptar Términos y Condiciones.");

            if (!vm.AceptaPrivacidad)
                ModelState.AddModelError(nameof(vm.AceptaPrivacidad), "Debes aceptar la Política de Privacidad.");

 
            SetSession("VOL_P3", new RegistroVoluntarioPaso3Sesion
            {
                Email = vm.Email,
                AceptaTerminos = vm.AceptaTerminos,
                AceptaPrivacidad = vm.AceptaPrivacidad
            });

            if (!ModelState.IsValid)
                return View(vm);

            var p1 = GetSession<RegistroVoluntarioPaso1VM>("VOL_P1");
            var p2 = GetSession<RegistroVoluntarioPaso2VM>("VOL_P2");

            if (p1 == null || p2 == null)
            {
                ModelState.AddModelError("", "Tu sesión expiró. Vuelve a iniciar el registro.");
                return View(vm);
            }

            var email = (vm.Email ?? "").Trim().ToLower();

            if (await _usuarioRepo.EmailExisteAsync(email))
            {
                ModelState.AddModelError(nameof(vm.Email), "Este correo ya está registrado.");
                return View(vm);
            }

            var passwordHash = _hasher.HashPassword(email, vm.Password);

            
            var userId = await _usuarioRepo.CrearUsuarioAsync(new UsuarioModel
            {
                Email = email,
                PasswordHash = passwordHash,
                Rol = "VOLUNTARIO",
                EsMayorEdad = p1.EsMayorEdad,
                FotoPerfilUrl = null
            });

           
            await _volRepo.CrearPerfilAsync(new VoluntarioPerfilModel
            {
                UsuarioId = userId,
                Nombres = p1.Nombres,
                Apellidos = p1.Apellidos,
                Sexo = p1.Sexo,
                Celular = p1.Celular,
                FechaNacimiento = p1.FechaNacimiento,
                Ciudad = "Lima",
            });

          
            await _volRepo.InsertarHabilidadesAsync(userId, p2.HabilidadesIds);

            RemoveSession("VOL_P1", "VOL_P2", "VOL_P3");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult OrganizacionPaso1()
        {
            var vm = GetSession<RegistroOrganizacionPaso1VM>("ORG_P1");
            return View(vm ?? new RegistroOrganizacionPaso1VM());
        }

        [HttpPost]
        public IActionResult OrganizacionPaso1(RegistroOrganizacionPaso1VM vm)
        {
            if (!vm.EsMayorEdad)
                ModelState.AddModelError(nameof(vm.EsMayorEdad), "Debes confirmar que eres mayor de edad.");

            if (!ModelState.IsValid)
                return View(vm);

            SetSession("ORG_P1", vm);
            return RedirectToAction(nameof(OrganizacionPaso2));
        }

        [HttpGet]
        public async Task<IActionResult> OrganizacionPaso2()
        {
            ViewBag.TiposOrganizacion = await _tipoOrgRepo.ListarAsync();


            return View(new RegistroOrganizacionPaso2VM());
        }

        [HttpPost]
        public async Task<IActionResult> OrganizacionPaso2(RegistroOrganizacionPaso2VM vm)
        {
            ViewBag.TiposOrganizacion = await _tipoOrgRepo.ListarAsync();

            if (vm.FotoPerfilFile == null || vm.FotoPerfilFile.Length == 0)
                ModelState.AddModelError(nameof(vm.FotoPerfilFile), "La foto de la organización es obligatoria.");

            if (vm.FotoPerfilFile != null)
            {
                var ct = (vm.FotoPerfilFile.ContentType ?? "").ToLower();
                if (ct != "image/jpeg" && ct != "image/png" && ct != "image/webp")
                    ModelState.AddModelError(nameof(vm.FotoPerfilFile), "Formato inválido. Use JPG, PNG o WEBP.");

                if (vm.FotoPerfilFile.Length > 2 * 1024 * 1024)
                    ModelState.AddModelError(nameof(vm.FotoPerfilFile), "Máximo 2MB.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "organizaciones");
            Directory.CreateDirectory(uploadsRoot);

            var ext = Path.GetExtension(vm.FotoPerfilFile!.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await vm.FotoPerfilFile.CopyToAsync(stream);

            var p2Sesion = new RegistroOrganizacionPaso2Sesion
            {
                FotoPerfilUrl = $"/uploads/organizaciones/{fileName}",
                NombreOrganizacion = vm.NombreOrganizacion,
                TipoOrganizacionId = vm.TipoOrganizacionId,
                DescripcionCorta = vm.DescripcionCorta
            };

            SetSession("ORG_P2", p2Sesion);
            return RedirectToAction(nameof(OrganizacionPaso3));
        }

        [HttpGet]
        public async Task<IActionResult> OrganizacionPaso3()
        {
            ViewBag.Distritos = await _distritoRepo.ListarActivosAsync();

            var vm = GetSession<RegistroOrganizacionPaso3VM>("ORG_P3");
            return View(vm ?? new RegistroOrganizacionPaso3VM());
        }

        [HttpPost]
        public async Task<IActionResult> OrganizacionPaso3(RegistroOrganizacionPaso3VM vm)
        {
            ViewBag.Distritos = await _distritoRepo.ListarActivosAsync();

            if (!ModelState.IsValid)
                return View(vm);

            SetSession("ORG_P3", vm);
            return RedirectToAction(nameof(OrganizacionPaso4));
        }

        [HttpGet]
        public IActionResult OrganizacionPaso4()
        {
            var vm = GetSession<RegistroOrganizacionPaso4VM>("ORG_P4");
            return View(vm ?? new RegistroOrganizacionPaso4VM());
        }

        [HttpPost]
        public async Task<IActionResult> OrganizacionPaso4(RegistroOrganizacionPaso4VM vm)
        {
            if (!vm.AceptaTerminos)
                ModelState.AddModelError(nameof(vm.AceptaTerminos), "Debes aceptar términos como organización.");

            if (!vm.AutorizaDatos)
                ModelState.AddModelError(nameof(vm.AutorizaDatos), "Debes autorizar el uso de datos.");

            SetSession("ORG_P4", vm);

            if (!ModelState.IsValid)
                return View(vm);

            var p1 = GetSession<RegistroOrganizacionPaso1VM>("ORG_P1");
            var p2 = GetSession<RegistroOrganizacionPaso2Sesion>("ORG_P2");
            var p3 = GetSession<RegistroOrganizacionPaso3VM>("ORG_P3");

            if (p1 == null || p2 == null || p3 == null)
            {
                ModelState.AddModelError("", "Tu sesión expiró. Vuelve a iniciar el registro.");
                return View(vm);
            }

            var email = (p1.Email ?? "").Trim().ToLower();

            if (await _usuarioRepo.EmailExisteAsync(email))
            {
                ModelState.AddModelError("", "Este correo ya está registrado.");
                return View(vm);
            }


            var tipoNombre = await _tipoOrgRepo.ObtenerNombreAsync(p2.TipoOrganizacionId);
            if (string.IsNullOrWhiteSpace(tipoNombre))
            {
                ModelState.AddModelError("", "El tipo de organización seleccionado no es válido.");
                return View(vm);
            }

            var passwordHash = _hasher.HashPassword(email, p1.Password);


            var userId = await _usuarioRepo.CrearUsuarioAsync(new UsuarioModel
            {
                Email = email,
                PasswordHash = passwordHash,
                Rol = "ORGANIZACION",
                EsMayorEdad = p1.EsMayorEdad,
                FotoPerfilUrl = p2.FotoPerfilUrl
            });

            await _orgRepo.CrearPerfilAsync(new OrganizacionPerfilModel
            {
                UsuarioId = userId,
                NombresResponsable = p1.NombresResponsable,
                ApellidosResponsable = p1.ApellidosResponsable,
                CelularResponsable = p1.CelularResponsable,

                NombreOrganizacion = p2.NombreOrganizacion,
                TipoOrganizacion = tipoNombre,
                DescripcionCorta = p2.DescripcionCorta,

                CorreoPublicoContacto = p3.CorreoPublicoContacto,
                CelularPublico = p3.CelularPublico,

                Ciudad = "Lima",
                Distrito = p3.Distrito,
                Direccion = p3.Direccion
            });

        
            if (!string.IsNullOrWhiteSpace(p3.FacebookUrl))
                await _orgRepo.InsertarRedSocialAsync(userId, "FB", p3.FacebookUrl);

            if (!string.IsNullOrWhiteSpace(p3.InstagramUrl))
                await _orgRepo.InsertarRedSocialAsync(userId, "IG", p3.InstagramUrl);

            if (!string.IsNullOrWhiteSpace(p3.TikTokUrl))
                await _orgRepo.InsertarRedSocialAsync(userId, "TT", p3.TikTokUrl);

            if (!string.IsNullOrWhiteSpace(p3.WebUrl))
                await _orgRepo.InsertarRedSocialAsync(userId, "WEB", p3.WebUrl);

            RemoveSession("ORG_P1", "ORG_P2", "ORG_P3", "ORG_P4");

            return RedirectToAction("Index", "Home");
        }
    }

    public class RegistroOrganizacionPaso2Sesion
    {
        public string FotoPerfilUrl { get; set; } = "";
        public string NombreOrganizacion { get; set; } = "";
        public int TipoOrganizacionId { get; set; }
        public string? DescripcionCorta { get; set; }
    }

    public class RegistroVoluntarioPaso3Sesion
    {
        public string? Email { get; set; }
        public bool AceptaTerminos { get; set; }
        public bool AceptaPrivacidad { get; set; }
    }
}
