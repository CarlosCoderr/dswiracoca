using System;
using System.Collections.Generic;

namespace Proyecto_DSWI.Models
{
    public class EventoDetalleModel
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string? Categoria { get; set; }
        public string? NombreOrganizacion { get; set; }

        public string? DescripcionCorta { get; set; }
        public string? DescripcionLarga { get; set; }

        public DateTime FechaEvento { get; set; }
        public TimeSpan? HoraEvento { get; set; }

        public string? Distrito { get; set; }
        public string? Direccion { get; set; }

        public string? MapaUrl { get; set; }

        public bool Ilimitado { get; set; }
        public int? CuposLimite { get; set; }

        public string? Requisitos { get; set; }
        public string? Incluye { get; set; }
        public string? NoIncluye { get; set; }

        public string? MetodoContacto { get; set; }
        public string? WhatsappNumero { get; set; }

        public string? ImagenPrincipal { get; set; }

        public List<string> ImagenesGaleria { get; set; } = new();
    }
}
