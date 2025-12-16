using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class HabilidadRepository
    {
        private readonly string _cn;

        public HabilidadRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("cnRacoca");
        }

        public async Task<List<HabilidadModel>> ListarAsync()
        {
            var list = new List<HabilidadModel>();

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SELECT id, nombre FROM habilidades ORDER BY nombre", conn);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new HabilidadModel
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return list;
        }
    }
}
