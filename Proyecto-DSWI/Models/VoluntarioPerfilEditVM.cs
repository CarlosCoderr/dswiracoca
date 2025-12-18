using System;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class VoluntarioPerfilEditVM
    {
        public int UsuarioId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; } = "";

        [Required, StringLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = "";

        [Display(Name = "Sexo")]
        public string? Sexo { get; set; }  

        [Phone]
        [Display(Name = "Celular")]
        public string? Celular { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [StringLength(100)]
        [Display(Name = "Ciudad")]
        public string? Ciudad { get; set; }

        [StringLength(100)]
        [Display(Name = "Distrito")]
        public string? Distrito { get; set; }

        [Display(Name = "Foto de perfil (URL o ruta)")]
        public string? FotoPerfilUrl { get; set; }
    }
}
