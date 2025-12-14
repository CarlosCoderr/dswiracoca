using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class DistritoRepository
    {
        private readonly string _cs;

        public DistritoRepository(IConfiguration config)
        {
            _cs = config.GetConnectionString("CibertecConnection")!;
        }

        private SqlConnection GetConn() => new SqlConnection(_cs);

        public Task<List<DistritoModel>> ListarActivosAsync()
            => ListarActivosAsyncInternal();

        private async Task<List<DistritoModel>> ListarActivosAsyncInternal()
        {
            var lista = new List<DistritoModel>();

            var sql = @"SELECT id, nombre FROM dbo.distritos WHERE activo = 1 ORDER BY nombre;";

            await using var conn = GetConn();
            await using var cmd = new SqlCommand(sql, conn);

            await conn.OpenAsync();
            await using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                lista.Add(new DistritoModel
                {
                    Id = rd.GetInt32(0),
                    Nombre = rd.GetString(1)
                });
            }

            return lista;
        }
    }
}
