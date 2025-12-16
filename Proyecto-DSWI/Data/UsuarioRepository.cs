using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class UsuarioRepository
    {
        private readonly string _cn;

        public UsuarioRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("cnRacoca")!;
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            const string sql = @"SELECT COUNT(1) FROM usuarios WHERE email = @email;";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);

            await conn.OpenAsync();
            var count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        public async Task<int> CrearUsuarioAsync(UsuarioModel usuario)
        {
            const string sql = @"
INSERT INTO usuarios (email, password_hash, rol, es_mayor_edad, estado, foto_perfil_url)
OUTPUT INSERTED.id
VALUES (@email, @password_hash, @rol, @es_mayor_edad, @estado, @foto);
";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@email", usuario.Email);
            cmd.Parameters.AddWithValue("@password_hash", usuario.PasswordHash);
            cmd.Parameters.AddWithValue("@rol", usuario.Rol);
            cmd.Parameters.AddWithValue("@es_mayor_edad", usuario.EsMayorEdad);
            cmd.Parameters.AddWithValue("@estado", string.IsNullOrWhiteSpace(usuario.Estado) ? "ACTIVO" : usuario.Estado);
            cmd.Parameters.AddWithValue("@foto", (object?)usuario.FotoPerfilUrl ?? DBNull.Value);

            await conn.OpenAsync();
            var newId = (int)await cmd.ExecuteScalarAsync();
            return newId;
        }

        public async Task<UsuarioModel?> ObtenerPorEmailAsync(string email)
        {
            const string sql = @"
SELECT id, email, password_hash, rol, estado, foto_perfil_url
FROM usuarios
WHERE email = @email;
";
            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);

            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            if (!await rd.ReadAsync()) return null;

            return new UsuarioModel
            {
                Id = rd.GetInt32(0),
                Email = rd.GetString(1),
                PasswordHash = rd.GetString(2),
                Rol = rd.GetString(3),
                Estado = rd.GetString(4),
                FotoPerfilUrl = rd.IsDBNull(5) ? null : rd.GetString(5)
            };
        }

        public async Task<string?> ObtenerNombreParaSaludoAsync(int usuarioId, string rol)
        {
            string sql = rol == "VOLUNTARIO"
                ? "SELECT TOP 1 nombres FROM voluntario_perfil WHERE usuario_id = @id"
                : "SELECT TOP 1 nombre_organizacion FROM organizacion_perfil WHERE usuario_id = @id";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", usuarioId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }
    }
}
