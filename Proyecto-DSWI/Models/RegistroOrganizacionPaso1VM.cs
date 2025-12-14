using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroOrganizacionPaso1VM
    {
        [Required] public string NombresResponsable { get; set; } = "";
        [Required] public string ApellidosResponsable { get; set; } = "";
        public string? CelularResponsable { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Display(Name = "Soy mayor de edad")]
        public bool EsMayorEdad { get; set; }
    }
}
