using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class MisInscripcionesRepository
    {
        private readonly string _cn;

        public MisInscripcionesRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task<List<MiInscripcionModel>> ListarPorVoluntarioAsync(int voluntarioId)
        {
            var list = new List<MiInscripcionModel>();

            const string sql = @"
SELECT 
    e.id,
    e.nombre,
    o.nombre_organizacion,
    e.fecha_evento,
    e.distrito,
    i.estado
FROM evento_inscripciones i
INNER JOIN eventos e ON e.id = i.evento_id
INNER JOIN organizacion_perfil o ON o.usuario_id = e.organizacion_id
WHERE i.voluntario_id = @vol
ORDER BY e.fecha_evento DESC;
";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@vol", voluntarioId);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new MiInscripcionModel
                {
                    EventoId = rd.GetInt32(0),
                    NombreEvento = rd.GetString(1),
                    Organizacion = rd.GetString(2),
                    FechaEvento = rd.GetDateTime(3),
                    Distrito = rd.GetString(4),
                    Estado = rd.GetString(5)
                });
            }

            return list;
        }
    }
}
