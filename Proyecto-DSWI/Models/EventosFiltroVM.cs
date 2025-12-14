namespace Proyecto_DSWI.Models
{
    public class EventosFiltroVM
    {
        public string? Q { get; set; }
        public string? Distrito { get; set; }
        public int? CategoriaId { get; set; }
        public DateTime? Fecha { get; set; }

        public List<EventoModel> Eventos { get; set; } = new();
        public List<CategoriaEventoModel> Categorias { get; set; } = new();
        public List<DistritoModel> Distritos { get; set; } = new();         
    }
}
