using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class TipoOrganizacionRepository
    {
        private readonly string _cn;

        public TipoOrganizacionRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task<string?> ObtenerNombreAsync(int id)
        {
            const string sql = "SELECT nombre FROM tipos_organizacion WHERE id = @id;";
            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }


        public async Task<List<TipoOrganizacionModel>> ListarAsync()
        {
            var list = new List<TipoOrganizacionModel>();

            const string sql = "SELECT id, nombre FROM tipos_organizacion ORDER BY nombre;";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                list.Add(new TipoOrganizacionModel
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return list;
        }
    }
}
