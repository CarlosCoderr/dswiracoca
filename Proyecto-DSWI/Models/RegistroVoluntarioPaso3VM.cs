using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroVoluntarioPaso3VM
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        public bool AceptaTerminos { get; set; }
        public bool AceptaPrivacidad { get; set; }
    }
}
