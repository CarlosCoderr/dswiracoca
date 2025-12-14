using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class EventoDetalleRepository
    {
        private readonly string _cn;

        public EventoDetalleRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task<EventoDetalleModel?> ObtenerAsync(int eventoId)
        {
            EventoDetalleModel? evento = null;

            const string sqlEvento = @"
SELECT 
    e.id,
    e.nombre,
    c.nombre AS categoria,
    o.nombre_organizacion,
    e.fecha_evento,
    e.hora_evento,
    e.distrito,
    e.direccion,
    e.descripcion_larga,
    e.requisitos,
    e.incluye,
    e.no_incluye,
    e.imagen_principal,
    e.mapa_url,
    e.whatsapp_numero
FROM eventos e
INNER JOIN categorias_evento c ON c.id = e.categoria_id
INNER JOIN organizacion_perfil o ON o.usuario_id = e.organizacion_id
WHERE e.id = @id AND e.estado = 'PUBLICADO';
";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sqlEvento, conn);
            cmd.Parameters.AddWithValue("@id", eventoId);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (await rd.ReadAsync())
            {
                evento = new EventoDetalleModel
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    Categoria = rd.GetString(2),
                    NombreOrganizacion = rd.GetString(3),
                    FechaEvento = rd.GetDateTime(4),
                    HoraEvento = rd.GetTimeSpan(5),
                    Distrito = rd.GetString(6),
                    Direccion = rd.GetString(7),
                    DescripcionLarga = rd.GetString(8),
                    Requisitos = rd.IsDBNull(9) ? null : rd.GetString(9),
                    Incluye = rd.IsDBNull(10) ? null : rd.GetString(10),
                    NoIncluye = rd.IsDBNull(11) ? null : rd.GetString(11),
                    ImagenPrincipal = rd.GetString(12),
                    MapaUrl = rd.IsDBNull(13) ? null : rd.GetString(13),
                    WhatsappNumero = rd.IsDBNull(14) ? null : rd.GetString(14)
                };
            }

            if (evento == null) return null;

            // Galería
            const string sqlImgs = @"SELECT url FROM evento_imagenes WHERE evento_id = @id;";
            using var cmdImgs = new SqlCommand(sqlImgs, conn);
            cmdImgs.Parameters.AddWithValue("@id", eventoId);

            using var rdImgs = await cmdImgs.ExecuteReaderAsync();
            while (await rdImgs.ReadAsync())
            {
                evento.ImagenesGaleria.Add(rdImgs.GetString(0));
            }

            return evento;
        }
    }
}
