using Racoca_DSWI.Models;
using System.Collections.Generic;

namespace Racoca_DSWI.Data
{
    public class EventoRepository
    {
        private static List<Evento> _eventos = new List<Evento>()
        {
            new Evento {
                Id = 1,
                Titulo = "Carrera 5K",
                Descripcion = "Una carrera solidaria abierta al público que busca incentivar el deporte y la vida saludable mientras se apoya a familias en situación vulnerable. El evento contará con categorías para principiantes y corredores avanzados.",
                Categoria = "Deportivo,Social",
                Ubicacion = "Lima",
                Organizacion = "Fundación Vida",
                Cupos = 30,
                Imagen = "/imagenes/evento1.jpg"
            },

            new Evento {
                Id = 2,
                Titulo = "Feria Cultural",
                Descripcion = "Un espacio dedicado al arte, la música, la gastronomía y las tradiciones de distintas regiones del país. La feria busca promover la identidad cultural mediante presentaciones artísticas y talleres creativos.",
                Categoria = "Cultural, Social",
                Ubicacion = "Cusco",
                Organizacion = "Cultura Viva",
                Cupos = 20,
                Imagen = "/imagenes/evento2.jpg"
            },

            new Evento {
                Id = 3,
                Titulo = "Taller de Programación",
                Descripcion = "Taller práctico dirigido a estudiantes y personas interesadas en iniciarse en el mundo del desarrollo de software. Se abordarán conceptos básicos de programación en C#, lógica computacional y estructuras de control.",
                Categoria = "Educativo, Social",
                Ubicacion = "Lima",
                Organizacion = "Tech Perú",
                Cupos = 15,
                Imagen = "/imagenes/evento3.jpg"
            },

            new Evento {
                Id = 4,
                Titulo = "Campaña Solidaria",
                Descripcion = "Jornada de donación y apoyo social orientada a recolectar alimentos, ropa y útiles escolares para comunidades de bajos recursos. El evento busca fortalecer la solidaridad, el trabajo en equipo y la conciencia social.",
                Categoria = "Social, Cultural",
                Ubicacion = "Arequipa",
                Organizacion = "Manos Unidas",
                Cupos = 25,
                Imagen = "/imagenes/evento4.jpg"
            },

            new Evento {
                Id = 5,
                Titulo = "Caminata Ecológica",
                Descripcion = "Actividad recreativa orientada a promover el cuidado del medio ambiente mediante una caminata en espacios naturales. Incluye charlas de concientización ambiental, recolección de residuos y actividades educativas.",
                Categoria = "Social, Educativo",
                Ubicacion = "Lima",
                Organizacion = "Eco Perú",
                Cupos = 40,
                Imagen = "/imagenes/evento5.jpg"
            },

            new Evento {
                Id = 6,
                Titulo = "Hackatón",
                Descripcion = "Evento intensivo de innovación tecnológica donde equipos multidisciplinarios desarrollarán soluciones digitales en un tiempo limitado. La hackatón fomenta la creatividad y el trabajo colaborativo.",
                Categoria = "Educativo, Social",
                Ubicacion = "Cusco",
                Organizacion = "Dev Perú",
                Cupos = 18,
                Imagen = "/imagenes/evento6.jpg"
            },

            new Evento {
                Id = 7,
                Titulo = "Maratón Juvenil",
                Descripcion = "Competencia deportiva dirigida a jóvenes que promueve la actividad física, el compañerismo y la disciplina. El evento contará con acompañamiento médico, zonas de hidratación y animación musical.",
                Categoria = "Deportivo, Cultural",
                Ubicacion = "Lima",
                Organizacion = "Sport Vida",
                Cupos = 22,
                Imagen = "/imagenes/evento7.jpg"
            },

            new Evento {
                Id = 8,
                Titulo = "Expo Artesanía",
                Descripcion = "Exposición dedicada a la difusión del talento artesanal peruano, donde emprendedores locales presentarán piezas hechas a mano, textiles, cerámica y productos tradicionales. Su objetivo es impulsar la economía local.",
                Categoria = "Cultural, Social",
                Ubicacion = "Arequipa",
                Organizacion = "Artes Perú",
                Cupos = 35,
                Imagen = "/imagenes/evento8.jpg"
            }

        };

        public List<Evento> ListarEventos()
        {
            return _eventos;
        }
    }
}
