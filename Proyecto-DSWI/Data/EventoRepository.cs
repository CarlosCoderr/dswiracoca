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

        public async Task<PagedResult<EventoModel>> ListarPaginadoAsync(
            string? q, string? distrito, int? categoriaId, DateTime? fecha,
            int page = 1, int pageSize = 3)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 3;

            var result = new PagedResult<EventoModel>
            {
                Page = page,
                PageSize = pageSize
            };

            var where = @" WHERE 1=1 ";
            if (!string.IsNullOrWhiteSpace(q)) where += " AND e.nombre LIKE @q ";
            if (!string.IsNullOrWhiteSpace(distrito)) where += " AND e.distrito = @distrito ";
            if (categoriaId.HasValue) where += " AND e.categoria_id = @categoriaId ";
            if (fecha.HasValue) where += " AND e.fecha_evento = @fecha ";

            var sqlCount = $@"
SELECT COUNT(1)
FROM dbo.eventos e
JOIN dbo.categorias_evento ce ON ce.id = e.categoria_id
JOIN dbo.organizacion_perfil op ON op.usuario_id = e.organizacion_id
{where};";

            var sqlData = $@"
SELECT
    e.id,
    e.categoria_id,
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
{where}
ORDER BY e.fecha_evento DESC, e.hora_evento DESC
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            await using var cn = new SqlConnection(_cn);
            await cn.OpenAsync();

            // COUNT
            await using (var cmd = new SqlCommand(sqlCount, cn))
            {
                if (!string.IsNullOrWhiteSpace(q)) cmd.Parameters.AddWithValue("@q", $"%{q.Trim()}%");
                if (!string.IsNullOrWhiteSpace(distrito)) cmd.Parameters.AddWithValue("@distrito", distrito.Trim());
                if (categoriaId.HasValue) cmd.Parameters.AddWithValue("@categoriaId", categoriaId.Value);
                if (fecha.HasValue) cmd.Parameters.AddWithValue("@fecha", fecha.Value.Date);

                result.Total = (int)await cmd.ExecuteScalarAsync();
            }

            // DATA
            await using (var cmd = new SqlCommand(sqlData, cn))
            {
                if (!string.IsNullOrWhiteSpace(q)) cmd.Parameters.AddWithValue("@q", $"%{q.Trim()}%");
                if (!string.IsNullOrWhiteSpace(distrito)) cmd.Parameters.AddWithValue("@distrito", distrito.Trim());
                if (categoriaId.HasValue) cmd.Parameters.AddWithValue("@categoriaId", categoriaId.Value);
                if (fecha.HasValue) cmd.Parameters.AddWithValue("@fecha", fecha.Value.Date);

                cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

                await using var rd = await cmd.ExecuteReaderAsync();
                while (await rd.ReadAsync())
                {
                    result.Items.Add(new EventoModel
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
            }

            return result;
        }

    }
}
