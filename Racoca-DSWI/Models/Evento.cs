namespace Racoca_DSWI.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        public string Categoria { get; set; }
        public string Ubicacion { get; set; }
        public string Organizacion { get; set; }

        public int Cupos { get; set; }

        public string Imagen { get; set; }
    }
}
