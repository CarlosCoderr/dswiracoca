namespace Proyecto_DSWI.Models
{
    public class MiInscripcionModel
    {
        public int EventoId { get; set; }
        public string NombreEvento { get; set; } = "";
        public string Organizacion { get; set; } = "";
        public DateTime FechaEvento { get; set; }
        public string Distrito { get; set; } = "";
        public string Estado { get; set; } = "";
    }
}
