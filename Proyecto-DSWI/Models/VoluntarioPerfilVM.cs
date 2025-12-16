namespace Proyecto_DSWI.Models
{
    public class VoluntarioPerfilVM
    {
        // usuarios
        public int UsuarioId { get; set; }
        public string Email { get; set; } = "";
        public string Rol { get; set; } = "";
        public string? FotoPerfilUrl { get; set; }
        public string Estado { get; set; } = "";

        // voluntario_perfil
        public string Nombres { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string? Sexo { get; set; }
        public string? Celular { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Ciudad { get; set; }
        public string? Distrito { get; set; }

        // habilidades
        public List<string> Habilidades { get; set; } = new();

        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    }
}
