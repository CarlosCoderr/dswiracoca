namespace Racoca_DSWI.Models
{
    public class Organizacion
    {
        public string NombreResponsable { get; set; } = "";
        public string ApellidoResponsable { get; set; } = "";
        public string CelularResponsable { get; set; } = "";
        public string CorreoResponsable { get; set; } = "";
        public string Contrasena { get; set; } = "";
        public bool EsMayorDeEdad { get; set; }

        public string NombreOrganizacion { get; set; } = "";
        public string TipoOrganizacion { get; set; } = "";
        public string DescripcionCorta { get; set; } = "";
        public IFormFile? FotoOrganizacion { get; set; }

        public string CorreoPublico { get; set; } = "";
        public string CelularPublico { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public string Distrito { get; set; } = "";
        public string Direccion { get; set; } = "";

        public bool UsaFacebook { get; set; }
        public bool UsaInstagram { get; set; }
        public bool UsaTikTok { get; set; }
        public bool UsaThreads { get; set; }

        public string? LinkFacebook { get; set; }
        public string? LinkInstagram { get; set; }
        public string? LinkTikTok { get; set; }
        public string? LinkThreads { get; set; }


        public bool AceptoTerminos { get; set; }
        public bool AutorizaDatos { get; set; }
    }
}
