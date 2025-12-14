using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI.Models
{
    public class RegistroOrganizacionPaso4VM
    {
        [Display(Name = "Acepto los términos como organización")]
        public bool AceptaTerminos { get; set; }

        [Display(Name = "Autorizo el uso de datos para gestión del voluntariado")]
        public bool AutorizaDatos { get; set; }
    }
}
