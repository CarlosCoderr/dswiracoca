using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        [Required]
        public string Rol { get; set; } = ""; 

        public bool EsMayorEdad { get; set; }

        public string Estado { get; set; } = "ACTIVO";

        public DateTime FechaCreacion { get; set; }

        public string? FotoPerfilUrl { get; set; } // Org requerida en backend, voluntario opcional
    }
}
