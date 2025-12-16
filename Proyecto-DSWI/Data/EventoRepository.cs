using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Proyecto_DSWI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proyecto_DSWI.Data
{
    public class EventoRepository
    {
        private readonly string _cn;

        public EventoRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("cnRacoca")!;
        }

        public async Task<List<EventoModel>> ListarAsync(string? q, string? distrito, int? categoriaId, DateTime? fecha)
        {
            var lista = new List<EventoModel>();

            var sql = @"
SELECT
    e.id,
    e.nombre,
    e.descripcion_corta,
    e.fecha_evento,
    e.hora_evento,
    e.distrito,
    e.direccion,
    e.imagen_principal,
    e.whatsapp_numero,
    ce.nombre AS categoria_nombre,
    op.nombre_organizacion
FROM dbo.eventos e
JOIN dbo.categorias_evento ce ON ce.id = e.categoria_id
JOIN dbo.organizacion_perfil op ON op.usuario_id = e.organizacion_id
WHERE 1=1
";

            if (!string.IsNullOrWhiteSpace(q))
                sql += " AND e.nombre LIKE @q ";

            if (!string.IsNullOrWhiteSpace(distrito))
                sql += " AND e.distrito = @distrito ";

            if (categoriaId.HasValue)
                sql += " AND e.categoria_id = @categoriaId ";

            if (fecha.HasValue)
                sql += " AND e.fecha_evento = @fecha ";

            sql += " ORDER BY e.fecha_evento DESC, e.hora_evento DESC;";

            await using var cn = new SqlConnection(_cn);
            await using var cmd = new SqlCommand(sql, cn);

            if (!string.IsNullOrWhiteSpace(q))
                cmd.Parameters.AddWithValue("@q", $"%{q.Trim()}%");

            if (!string.IsNullOrWhiteSpace(distrito))
                cmd.Parameters.AddWithValue("@distrito", distrito.Trim());

            if (categoriaId.HasValue)
                cmd.Parameters.AddWithValue("@categoriaId", categoriaId.Value);

            if (fecha.HasValue)
                cmd.Parameters.AddWithValue("@fecha", fecha.Value.Date);

            await cn.OpenAsync();
            await using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                lista.Add(new EventoModel
                {
                    CategoriaId = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    DescripcionCorta = rd.IsDBNull(2) ? null : rd.GetString(2),
                    FechaEvento = rd.GetDateTime(3),
                    HoraEvento = rd.GetTimeSpan(4),
                    Distrito = rd.IsDBNull(5) ? null : rd.GetString(5),
                    Direccion = rd.IsDBNull(6) ? null : rd.GetString(6),
                    ImagenPrincipal = rd.IsDBNull(7) ? null : rd.GetString(7),
                    WhatsappNumero = rd.IsDBNull(8) ? null : rd.GetString(8),
                    CategoriaNombre = rd.IsDBNull(9) ? null : rd.GetString(9),
                    NombreOrganizacion = rd.IsDBNull(10) ? null : rd.GetString(10)
                });
            }

            return lista;
        }
    }
}
