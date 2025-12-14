using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroOrganizacionPaso3VM
    {
        [Required, EmailAddress]
        public string CorreoPublicoContacto { get; set; } = "";

        public string? CelularPublico { get; set; }

        [Required] public string Distrito { get; set; } = "";
        [Required] public string Direccion { get; set; } = "";

        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? WebUrl { get; set; }
    }
}
