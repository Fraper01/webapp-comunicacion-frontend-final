using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using WebApicomuniCancion.Models.Entities;
using Microsoft.Extensions.Configuration;
using WebApicomuniCancion.Interfaces;
using System.Linq; // Necesario para .Contains()

namespace WebApicomuniCancion.Services
{
    public class CitasDbService : BaseDbService, ICitasDbService
    {
        // Constructor modificado para recibir IConfiguration
        public CitasDbService(IConfiguration configuration) : base(configuration)
        {
            // El constructor de BaseDbService ahora también debe recibir IConfiguration
            // y pasarla a su propio constructor.
        }

        private Cita MapCitaFromReader(MySqlDataReader reader)
        {
            return new Cita
            {
                Id_Citas = reader.GetInt32(reader.GetOrdinal("id_citas")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                Tipo_Paciente = reader.GetString(reader.GetOrdinal("tipo_paciente")), 
                Tipo_Tratamiento = reader.GetString(reader.GetOrdinal("tipo_tratamiento")), 
                Turno_Manana = reader.GetBoolean(reader.GetOrdinal("turno_manana")),
                Turno_Tarde = reader.GetBoolean(reader.GetOrdinal("turno_tarde")),
                Turno_Noche = reader.GetBoolean(reader.GetOrdinal("turno_noche")),
                Turno_Sabado = reader.GetBoolean(reader.GetOrdinal("turno_sabado")),
                Desea_Alta_Usuario = reader.GetBoolean(reader.GetOrdinal("desea_alta_usuario")),
                Desea_Recibir_Novedades = reader.GetBoolean(reader.GetOrdinal("desea_recibir_novedades")),
                Fecha_Solicitud = reader.IsDBNull(reader.GetOrdinal("fecha_solicitud"))
                                            ? (DateTime?)null
                                            : (DateTime?)reader.GetValue(reader.GetOrdinal("fecha_solicitud")),
                Estatus = reader.GetString(reader.GetOrdinal("estatus"))
            };
        }

        public async Task<List<Cita>> GetAllCitasAsync()
        {
            var sql = "SELECT id_citas, nombre, apellido, email, telefono, tipo_paciente, tipo_tratamiento, turno_manana, turno_tarde, turno_noche, turno_sabado, desea_alta_usuario, desea_recibir_novedades, fecha_solicitud, estatus FROM citas";

            return await ExecuteReaderListAsync(sql, async (reader) => await Task.FromResult(MapCitaFromReader(reader)));
        }

        public async Task<Cita?> GetCitaByIdAsync(int id)
        {
            var sql = "SELECT id_citas, nombre, apellido, email, telefono, tipo_paciente, tipo_tratamiento, turno_manana, turno_tarde, turno_noche, turno_sabado, desea_alta_usuario, desea_recibir_novedades, fecha_solicitud, estatus FROM citas WHERE Id_citas = @ID"; 
            // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma

            return await ExecuteReaderAsync<Cita>(sql,
                (command) => command.Parameters.AddWithValue("@ID", id),
                async (reader) => 
                {
                    if (await reader.ReadAsync())
                    {
                        return MapCitaFromReader(reader); 
                    }
                    return null;
                });
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }

        public async Task AddCitaAsync(Cita cita)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand(
                    "INSERT INTO citas (nombre, apellido, email, telefono, tipo_paciente, tipo_tratamiento, turno_manana, turno_tarde, turno_noche, turno_sabado, desea_alta_usuario, desea_recibir_novedades, fecha_solicitud, estatus) " +
                    "VALUES (@nombre, @apellido, @email, @telefono, @tipoPaciente, @tipoTratamiento, @turnoManana, @turnoTarde, @turnoNoche, @turnoSabado, @deseaAltaUsuario, @deseaRecibirNovedades, @fechaSolicitud, @estatus)", 
                    connection))
                {
                    command.Parameters.AddWithValue("@nombre", cita.Nombre);
                    command.Parameters.AddWithValue("@apellido", cita.Apellido);
                    command.Parameters.AddWithValue("@email", cita.Email);
                    command.Parameters.AddWithValue("@telefono", cita.Telefono);
                    command.Parameters.AddWithValue("@tipoPaciente", cita.Tipo_Paciente);
                    command.Parameters.AddWithValue("@tipoTratamiento", cita.Tipo_Tratamiento);
                    command.Parameters.AddWithValue("@turnoManana", cita.Turno_Manana);
                    command.Parameters.AddWithValue("@turnoTarde", cita.Turno_Tarde);
                    command.Parameters.AddWithValue("@turnoNoche", cita.Turno_Noche);
                    command.Parameters.AddWithValue("@turnoSabado", cita.Turno_Sabado);
                    command.Parameters.AddWithValue("@deseaAltaUsuario", cita.Desea_Alta_Usuario);
                    command.Parameters.AddWithValue("@deseaRecibirNovedades", cita.Desea_Recibir_Novedades);
                    command.Parameters.AddWithValue("@fechaSolicitud", cita.Fecha_Solicitud);
                    command.Parameters.AddWithValue("@estatus", cita.Estatus);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task UpdateCitaAsync(Cita cita)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "UPDATE citas SET nombre = @nombre, apellido = @apellido," +
                    "email = @email, telefono = @telefono, " +
                    "tipo_paciente = @tipoPaciente, tipo_tratamiento = @TipoTratamiento," +
                    "turno_manana = @turnoManana, turno_tarde = @turnoTarde," +
                    "turno_noche = @turnoNoche, turno_sabado = @turnoSabado," +
                    "desea_alta_usuario = @deseaAltaUsuario, desea_recibir_novedades = @deseaRecibirNovedades," +
                    "fecha_solicitud = @fechaSolicitud," +
                    "estatus = @estatus" +
                    " WHERE id_citas = @ID", 
                    connection))
                {
                    command.Parameters.AddWithValue("@nombre", cita.Nombre);
                    command.Parameters.AddWithValue("@apellido", cita.Apellido);
                    command.Parameters.AddWithValue("@email", cita.Email);
                    command.Parameters.AddWithValue("@telefono", cita.Telefono);
                    command.Parameters.AddWithValue("@tipoPaciente", cita.Tipo_Paciente);
                    command.Parameters.AddWithValue("@tipoTratamiento", cita.Tipo_Tratamiento);
                    command.Parameters.AddWithValue("@turnoManana", cita.Turno_Manana);
                    command.Parameters.AddWithValue("@turnoTarde", cita.Turno_Tarde);
                    command.Parameters.AddWithValue("@turnoNoche", cita.Turno_Noche);
                    command.Parameters.AddWithValue("@turnoSabado", cita.Turno_Sabado);
                    command.Parameters.AddWithValue("@deseaAltaUsuario", cita.Desea_Alta_Usuario);
                    command.Parameters.AddWithValue("@deseaRecibirNovedades", cita.Desea_Recibir_Novedades);
                    command.Parameters.AddWithValue("@fechaSolicitud", cita.Fecha_Solicitud);
                    command.Parameters.AddWithValue("@estatus", cita.Estatus);
                    command.Parameters.AddWithValue("@ID", cita.Id_Citas);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }


        public async Task<bool> UpdateCitaStatusAsync(int id, string newStatus)
        {
            var sql = "UPDATE citas SET estatus = @estatus WHERE id_citas = @ID";
            var rowsAffected = await ExecuteNonQueryWithRowsAffectedAsync(async (connection) =>
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@estatus", newStatus);
                    command.Parameters.AddWithValue("@ID", id);
                    return await command.ExecuteNonQueryAsync(); // command.ExecuteNonQueryAsync() SÍ devuelve int
                }
            });
            return rowsAffected > 0; // Retorna true si se actualizó al menos una fila
        }

        public async Task DeleteCitaAsync(int id)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand("DELETE FROM citas WHERE id_citas = @ID", connection)) 
                {
                    command.Parameters.AddWithValue("@ID", id);
                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }
    }
}
