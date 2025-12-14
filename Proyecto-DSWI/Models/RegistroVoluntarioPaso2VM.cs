using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroVoluntarioPaso2VM
    {
        [MinLength(3, ErrorMessage = "Selecciona mínimo 3 habilidades.")]
        public List<int> HabilidadesIds { get; set; } = new();
    }
}
