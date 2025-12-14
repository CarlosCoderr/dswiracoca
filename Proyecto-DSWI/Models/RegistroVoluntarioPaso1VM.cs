using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroVoluntarioPaso1VM
    {
        [Required] public string Nombres { get; set; } = "";
        [Required] public string Apellidos { get; set; } = "";

        public string? Sexo { get; set; }
        public string? Celular { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Soy mayor de edad")]
        public bool EsMayorEdad { get; set; }
    }
}
