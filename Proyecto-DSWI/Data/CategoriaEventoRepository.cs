using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class CategoriaEventoRepository
    {
        private readonly string _cn;

        public CategoriaEventoRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task<List<CategoriaEventoModel>> ListarAsync()
        {
            var list = new List<CategoriaEventoModel>();

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SELECT id, nombre FROM categorias_evento ORDER BY nombre", conn);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new CategoriaEventoModel
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return list;
        }
    }
}
