using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class EventoModel
    {
        [Required]
        public int CategoriaId { get; set; }

        public string? CategoriaNombre { get; set; }
        public string? NombreOrganizacion { get; set; }

        [Required, StringLength(150)]
        public string Nombre { get; set; } = "";

        [StringLength(300)]
        public string? DescripcionCorta { get; set; }

        public string? DescripcionLarga { get; set; }

        [Required]
        public DateTime FechaEvento { get; set; } = DateTime.Today;

        public TimeSpan? HoraEvento { get; set; }

        [Required, StringLength(100)]
        public string Distrito { get; set; } = "";

        [Required, StringLength(200)]
        public string Direccion { get; set; } = "";

        public string? Requisitos { get; set; }
        public string? Incluye { get; set; }
        public string? NoIncluye { get; set; }

        [Required, StringLength(500)]
        public string ImagenPrincipal { get; set; } = "";

        public string? MapaUrl { get; set; }
        public string? WhatsappNumero { get; set; }

        public string? MetodoContacto { get; set; } = "WhatsApp";
        public string Estado { get; set; } = "Activo";

        public bool Ilimitado { get; set; } = true;
        public int? CuposLimite { get; set; }

        public string? ImagenGaleria1 { get; set; }
        public string? ImagenGaleria2 { get; set; }
        public string? ImagenGaleria3 { get; set; }
    }
}
