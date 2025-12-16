using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class CrearEventoModel
    {
        [Required]
        public int CategoriaId { get; set; }

        [Required, StringLength(200)]
        public string Nombre { get; set; } = "";

        [StringLength(500)]
        public string? DescripcionCorta { get; set; }

        public string? DescripcionLarga { get; set; }

        [Required]
        public DateTime FechaEvento { get; set; } = DateTime.Today;

        // Si tu formulario usa input type="time", se mapeará a TimeSpan
        public TimeSpan? HoraEvento { get; set; }

        // En tu BD eventos.distrito es NVARCHAR, así que lo guardamos como texto
        public string? Distrito { get; set; }

        public string? Direccion { get; set; }
        public string? MapaUrl { get; set; }

        public string? Requisitos { get; set; }
        public string? Incluye { get; set; }
        public string? NoIncluye { get; set; }

        public string? WhatsappNumero { get; set; }
        public string? MetodoContacto { get; set; }

        public string Estado { get; set; } = "Activo";

        // Cupos
        public bool Ilimitado { get; set; } = true;
        public int? CuposLimite { get; set; }

        public IFormFile? ImagenPrincipalFile { get; set; }

        // Máx 15 (validas en Controller)
        public List<IFormFile>? GaleriaFiles { get; set; }
    }
}
