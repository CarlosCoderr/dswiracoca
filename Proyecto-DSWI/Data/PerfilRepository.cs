using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class PerfilRepository
    {
        private readonly string _cn;
        public PerfilRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("cnRacoca") ?? "";
        }

        public VoluntarioPerfilVM? ObtenerVoluntario(int id)
        {
            if (string.IsNullOrWhiteSpace(_cn)) return null;

            using var con = new SqlConnection(_cn);
            con.Open();

            VoluntarioPerfilVM? vm = null;

            using (var cmd = new SqlCommand(@"
SELECT u.id, u.email, u.rol, u.estado, u.foto_perfil_url,
       vp.nombres, vp.apellidos, vp.sexo, vp.celular, vp.fecha_nacimiento,
       vp.ciudad, vp.distrito
FROM dbo.usuarios u
JOIN dbo.voluntario_perfil vp ON vp.usuario_id = u.id
WHERE u.id = @id AND u.rol = N'VOLUNTARIO';
", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var rd = cmd.ExecuteReader();
                if (!rd.Read()) return null;

                vm = new VoluntarioPerfilVM
                {
                    UsuarioId = rd.GetInt32(0),
                    Email = rd.GetString(1),
                    Rol = rd.GetString(2),
                    Estado = rd.GetString(3),
                    FotoPerfilUrl = rd.IsDBNull(4) ? null : rd.GetString(4),
                    Nombres = rd.GetString(5),
                    Apellidos = rd.GetString(6),
                    Sexo = rd.IsDBNull(7) ? null : rd.GetString(7),
                    Celular = rd.IsDBNull(8) ? null : rd.GetString(8),
                    FechaNacimiento = rd.IsDBNull(9) ? null : rd.GetDateTime(9),
                    Ciudad = rd.IsDBNull(10) ? null : rd.GetString(10),
                    Distrito = rd.IsDBNull(11) ? null : rd.GetString(11)
                };
            }

            using (var cmdHab = new SqlCommand(@"
SELECT h.nombre
FROM dbo.voluntario_habilidad vh
JOIN dbo.habilidades h ON h.id = vh.habilidad_id
WHERE vh.usuario_id = @id
ORDER BY h.nombre;", con))
            {
                cmdHab.Parameters.AddWithValue("@id", id);
                using var rdHab = cmdHab.ExecuteReader();
                while (rdHab.Read())
                    vm!.Habilidades.Add(rdHab.GetString(0));
            }

            return vm;
        }

        // === OBTENER DATOS PARA EDITAR PERFIL ===
        public VoluntarioPerfilEditVM? ObtenerVoluntarioParaEditar(int id)
        {
            if (string.IsNullOrWhiteSpace(_cn)) return null;

            using var con = new SqlConnection(_cn);
            con.Open();

            using var cmd = new SqlCommand(@"
SELECT u.id,
       u.foto_perfil_url,
       vp.nombres,
       vp.apellidos,
       vp.sexo,
       vp.celular,
       vp.fecha_nacimiento,
       vp.ciudad,
       vp.distrito
FROM dbo.usuarios u
JOIN dbo.voluntario_perfil vp ON vp.usuario_id = u.id
WHERE u.id = @id AND u.rol = N'VOLUNTARIO';
", con);

            cmd.Parameters.AddWithValue("@id", id);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new VoluntarioPerfilEditVM
            {
                UsuarioId = rd.GetInt32(0),
                FotoPerfilUrl = rd.IsDBNull(1) ? null : rd.GetString(1),
                Nombres = rd.GetString(2),
                Apellidos = rd.GetString(3),
                Sexo = rd.IsDBNull(4) ? null : rd.GetString(4),
                Celular = rd.IsDBNull(5) ? null : rd.GetString(5),
                FechaNacimiento = rd.IsDBNull(6) ? null : rd.GetDateTime(6),
                Ciudad = rd.IsDBNull(7) ? null : rd.GetString(7),
                Distrito = rd.IsDBNull(8) ? null : rd.GetString(8)
            };
        }

        // === ACTUALIZAR PERFIL EN BD ===
        public void ActualizarVoluntario(VoluntarioPerfilEditVM model)
        {
            if (string.IsNullOrWhiteSpace(_cn)) return;

            using var con = new SqlConnection(_cn);
            con.Open();

            using var tx = con.BeginTransaction();
            try
            {
                // 1) voluntario_perfil
                using (var cmd = new SqlCommand(@"
UPDATE dbo.voluntario_perfil
SET nombres = @nombres,
    apellidos = @apellidos,
    sexo = @sexo,
    celular = @celular,
    fecha_nacimiento = @fechaNacimiento,
    ciudad = @ciudad,
    distrito = @distrito
WHERE usuario_id = @id;
", con, tx))
                {
                    cmd.Parameters.AddWithValue("@id", model.UsuarioId);
                    cmd.Parameters.AddWithValue("@nombres", model.Nombres);
                    cmd.Parameters.AddWithValue("@apellidos", model.Apellidos);
                    cmd.Parameters.AddWithValue("@sexo", (object?)model.Sexo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@celular", (object?)model.Celular ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@fechaNacimiento", (object?)model.FechaNacimiento ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ciudad", (object?)model.Ciudad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@distrito", (object?)model.Distrito ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                // 2) usuarios (foto_perfil_url)
                using (var cmd2 = new SqlCommand(@"
UPDATE dbo.usuarios
SET foto_perfil_url = @foto
WHERE id = @id;
", con, tx))
                {
                    cmd2.Parameters.AddWithValue("@id", model.UsuarioId);
                    cmd2.Parameters.AddWithValue("@foto", (object?)model.FotoPerfilUrl ?? DBNull.Value);
                    cmd2.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
