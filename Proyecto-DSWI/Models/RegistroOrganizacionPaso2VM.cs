using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroOrganizacionPaso2VM
    {
        [Required(ErrorMessage = "La foto de la organización es obligatoria.")]
        public IFormFile? FotoPerfilFile { get; set; }

        [Required(ErrorMessage = "El nombre de la organización es obligatorio.")]
        public string NombreOrganizacion { get; set; } = "";

        [Required(ErrorMessage = "Seleccione un tipo de organización.")]
        public int TipoOrganizacionId { get; set; }

        [MaxLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        public string? DescripcionCorta { get; set; }
    }
}
