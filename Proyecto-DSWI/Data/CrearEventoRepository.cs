using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Proyecto_DSWI.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Proyecto_DSWI.Data
{
    public class CrearEventoRepository
    {
        private readonly string _cn;

        public CrearEventoRepository(IConfiguration config)
        {
            _cn = config.GetConnectionString("CibertecConnection") ?? "";
        }

        public async Task<int> CrearEventoAsync(int organizacionId, CrearEventoModel model)
        {
            await using var cn = new SqlConnection(_cn);
            await cn.OpenAsync();

            int? cupos = model.Ilimitado ? null : (model.CuposLimite.HasValue && model.CuposLimite.Value > 0 ? model.CuposLimite : null);

            var sql = @"
INSERT INTO dbo.eventos
(
  organizacion_id,
  categoria_id,
  nombre,
  descripcion_corta,
  descripcion_larga,
  fecha_evento,
  hora_evento,
  ciudad,
  distrito,
  direccion,
  mapa_url,
  requisitos,
  incluye,
  no_incluye,
  whatsapp_numero,
  metodo_contacto,
  ilimitado,
  cupos_limite,
  imagen_principal
)
OUTPUT INSERTED.id
VALUES
(
  @organizacion_id,
  @categoria_id,
  @nombre,
  @descripcion_corta,
  @descripcion_larga,
  @fecha_evento,
  @hora_evento,
  @ciudad,
  @distrito,
  @direccion,
  @mapa_url,
  @requisitos,
  @incluye,
  @no_incluye,
  @whatsapp_numero,
  @metodo_contacto,
  @ilimitado,
  @cupos_limite,
  NULL
);";

            await using var cmd = new SqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@organizacion_id", organizacionId);
            cmd.Parameters.AddWithValue("@categoria_id", model.CategoriaId);

            cmd.Parameters.AddWithValue("@nombre", model.Nombre?.Trim() ?? "");
            cmd.Parameters.AddWithValue("@descripcion_corta", (object?)model.DescripcionCorta?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion_larga", (object?)model.DescripcionLarga?.Trim() ?? DBNull.Value);

            cmd.Parameters.Add(new SqlParameter("@fecha_evento", SqlDbType.Date) { Value = model.FechaEvento.Date });

            cmd.Parameters.Add(new SqlParameter("@hora_evento", SqlDbType.Time)
            {
                Value = (object?)model.HoraEvento ?? DBNull.Value
            });

            cmd.Parameters.AddWithValue("@ciudad", "Lima");

            cmd.Parameters.AddWithValue("@distrito", (object?)model.Distrito?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)model.Direccion?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mapa_url", (object?)model.MapaUrl?.Trim() ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@requisitos", (object?)model.Requisitos?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@incluye", (object?)model.Incluye?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@no_incluye", (object?)model.NoIncluye?.Trim() ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@whatsapp_numero", (object?)model.WhatsappNumero?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@metodo_contacto", (object?)model.MetodoContacto?.Trim() ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@ilimitado", model.Ilimitado);
            cmd.Parameters.AddWithValue("@cupos_limite", (object?)cupos ?? DBNull.Value);

            var newId = (int)await cmd.ExecuteScalarAsync();
            return newId;
        }

        public async Task ActualizarImagenPrincipalAsync(int eventoId, string imagenUrl)
        {
            await using var cn = new SqlConnection(_cn);
            await cn.OpenAsync();

            var sql = @"UPDATE dbo.eventos SET imagen_principal = @url WHERE id = @id;";
            await using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", eventoId);
            cmd.Parameters.AddWithValue("@url", imagenUrl);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task InsertarImagenGaleriaAsync(int eventoId, string imagenUrl)
        {
            await using var cn = new SqlConnection(_cn);
            await cn.OpenAsync();

            var sql = @"INSERT INTO dbo.evento_imagenes (evento_id, imagen_url) VALUES (@evento_id, @url);";
            await using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@evento_id", eventoId);
            cmd.Parameters.AddWithValue("@url", imagenUrl);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
