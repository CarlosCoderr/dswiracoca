using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo inválido.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = "";

        public string? ReturnUrl { get; set; }
    }
}
