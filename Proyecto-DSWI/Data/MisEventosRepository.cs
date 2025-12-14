using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class MisEventosRepository
    {
        private readonly string _cn;

        public MisEventosRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task<List<MisEventoModel>> ListarPorOrganizacionAsync(int organizacionId)
        {
            var list = new List<MisEventoModel>();

            const string sql = @"
SELECT 
    e.id,
    e.nombre,
    e.fecha_evento,
    e.distrito,
    e.estado,
    COUNT(i.id) AS inscritos
FROM eventos e
LEFT JOIN evento_inscripciones i ON i.evento_id = e.id
WHERE e.organizacion_id = @org
GROUP BY e.id, e.nombre, e.fecha_evento, e.distrito, e.estado
ORDER BY e.fecha_evento DESC;
";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@org", organizacionId);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new MisEventoModel
                {
                    EventoId = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    FechaEvento = rd.GetDateTime(2),
                    Distrito = rd.GetString(3),
                    Estado = rd.GetString(4),
                    TotalInscritos = rd.GetInt32(5)
                });
            }

            return list;
        }
    }
}
