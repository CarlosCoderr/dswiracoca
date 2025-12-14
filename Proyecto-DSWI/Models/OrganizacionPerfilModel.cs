using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class OrganizacionPerfilModel
    {
        public int UsuarioId { get; set; }

        [Required]
        public string NombresResponsable { get; set; } = "";

        [Required]
        public string ApellidosResponsable { get; set; } = "";

        public string? CelularResponsable { get; set; }

        [Required]
        public string NombreOrganizacion { get; set; } = "";

        [Required]
        public string TipoOrganizacion { get; set; } = "";

        public string? DescripcionCorta { get; set; }

        public string? Ciudad { get; set; } = "Lima";
        public string? Distrito { get; set; }
        public string? Direccion { get; set; }

        public string CorreoPublicoContacto { get; set; } = "";
        public string? CelularPublico { get; set; }
    }
}
