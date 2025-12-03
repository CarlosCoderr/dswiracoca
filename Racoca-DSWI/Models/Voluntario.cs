using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Racoca_DSWI.Models
{
    public class Voluntario
    {
        // Paso1
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^\\d+$", ErrorMessage = "El celular debe ser numérico")]
        public string Celular { get; set; } = string.Empty;
        [Required]
        public string Cuidad { get; set; } = string.Empty;
        [Required]
        public string Distrito { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }
        public bool MayorDeEdad { get; set; }

        // Paso2
        public List<string> ActividadesSeleccionadas { get; set; } = new();
        public string? Habilidades { get; set; }

        // Paso3
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool AceptoTerminos { get; set; }
        public bool AceptoPrivacidad { get; set; }
    }
}