using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class VoluntarioPerfilModel
    {
        public int UsuarioId { get; set; }

        [Required]
        public string Nombres { get; set; } = "";

        [Required]
        public string Apellidos { get; set; } = "";

        public string? Sexo { get; set; }
        public string? Celular { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        public string? Ciudad { get; set; } = "Lima";
        public string? Distrito { get; set; }
    }
}
