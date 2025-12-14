namespace Proyecto_DSWI.Models
{
    public class MisEventoModel
    {
        public int EventoId { get; set; }
        public string Nombre { get; set; } = "";
        public DateTime FechaEvento { get; set; }
        public string Distrito { get; set; } = "";
        public string Estado { get; set; } = "";
        public int TotalInscritos { get; set; }
    }
}
