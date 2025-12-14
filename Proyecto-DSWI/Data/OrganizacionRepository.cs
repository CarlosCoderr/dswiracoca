using Microsoft.Data.SqlClient;
using Proyecto_DSWI.Models;

namespace Proyecto_DSWI.Data
{
    public class OrganizacionRepository
    {
        private readonly string _cn;

        public OrganizacionRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection");
        }

        public async Task CrearPerfilAsync(OrganizacionPerfilModel p)
        {
            const string sql = @"
INSERT INTO organizacion_perfil (
    usuario_id, nombres_responsable, apellidos_responsable, celular_responsable,
    nombre_organizacion, tipo_organizacion, descripcion_corta,
    ciudad, distrito, direccion
) VALUES (
    @uid, @nr, @ar, @cr,
    @no, @to, @dc,
    @ciudad, @dist, @dir
);";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@uid", p.UsuarioId);
            cmd.Parameters.AddWithValue("@nr", p.NombresResponsable);
            cmd.Parameters.AddWithValue("@ar", p.ApellidosResponsable);
            cmd.Parameters.AddWithValue("@cr", (object?)p.CelularResponsable ?? System.DBNull.Value);

            cmd.Parameters.AddWithValue("@no", p.NombreOrganizacion);
            cmd.Parameters.AddWithValue("@to", p.TipoOrganizacion);
            cmd.Parameters.AddWithValue("@dc", (object?)p.DescripcionCorta ?? System.DBNull.Value);

            cmd.Parameters.AddWithValue("@ciudad", (object?)p.Ciudad ?? "Lima");
            cmd.Parameters.AddWithValue("@dist", (object?)p.Distrito ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@dir", (object?)p.Direccion ?? System.DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task InsertarRedSocialAsync(int usuarioId, string plataforma, string url)
        {
            const string sql = @"
INSERT INTO organizacion_redes_sociales (usuario_id, plataforma, url)
VALUES (@uid, @pl, @url);";

            using var conn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@uid", usuarioId);
            cmd.Parameters.AddWithValue("@pl", plataforma);
            cmd.Parameters.AddWithValue("@url", url);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
