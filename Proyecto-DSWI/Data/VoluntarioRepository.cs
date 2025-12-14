using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class VoluntarioRepository
    {
        private readonly string _cn;

        public VoluntarioRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task CrearPerfilAsync(VoluntarioPerfilModel p)
        {
            const string sql = @"
INSERT INTO voluntario_perfil (usuario_id, nombres, apellidos, sexo, celular, fecha_nacimiento, ciudad, distrito)
VALUES (@uid, @n, @a, @sexo, @cel, @fn, @ciudad, @dist);";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@uid", p.UsuarioId);
            cmd.Parameters.AddWithValue("@n", p.Nombres);
            cmd.Parameters.AddWithValue("@a", p.Apellidos);
            cmd.Parameters.AddWithValue("@sexo", (object?)p.Sexo ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@cel", (object?)p.Celular ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@fn", (object?)p.FechaNacimiento ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@ciudad", (object?)p.Ciudad ?? "Lima");
            cmd.Parameters.AddWithValue("@dist", (object?)p.Distrito ?? System.DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task InsertarHabilidadesAsync(int usuarioId, List<int> habilidadesIds)
        {
            const string sql = @"INSERT INTO voluntario_habilidad (usuario_id, habilidad_id) VALUES (@uid, @hid);";

            using var conn = new SqlConnection(_cn);
            await conn.OpenAsync();

            foreach (var hid in habilidadesIds)
            {
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@uid", usuarioId);
                cmd.Parameters.AddWithValue("@hid", hid);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
