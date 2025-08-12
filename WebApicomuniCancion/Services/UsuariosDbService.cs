using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient; 
using System.Threading.Tasks;
using WebApicomuniCancion.Interfaces;
using WebApicomuniCancion.Models.Entities;
using Microsoft.Extensions.Configuration; 

namespace WebApicomuniCancion.Services
{
    public class UsuariosDbService : BaseDbService, IUsuariosDbService
    {
        public UsuariosDbService(IConfiguration configuration) : base(configuration)
        {
        }

        private Usuarios MapUsuarioFromReader(MySqlDataReader reader) 
        {
            return new Usuarios
            {
                id_user = reader.GetInt32(reader.GetOrdinal("id_user")),
                full_name = reader.GetString(reader.GetOrdinal("full_name")),
                user = reader.GetString(reader.GetOrdinal("user")),
                password = reader.GetString(reader.GetOrdinal("password")),
                fecha_registro = reader.IsDBNull(reader.GetOrdinal("fecha_registro"))
                                            ? (DateTime?)null
                                            : (DateTime?)reader.GetValue(reader.GetOrdinal("fecha_registro")),
                Usuario_Crea = reader.IsDBNull(reader.GetOrdinal("Usuario_Crea"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Usuario_Crea")),
                Equipo_Crea = reader.IsDBNull(reader.GetOrdinal("Equipo_Crea"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Equipo_Crea"))
            };
        }

        public async Task<List<Usuarios>> GetAllUsuariosAsync()
        {
            var sql = "SELECT id_user, full_name, user, password, fecha_registro, Usuario_Crea, Equipo_Crea FROM Usuarios";

            return await ExecuteReaderListAsync(sql, async (reader) => await Task.FromResult(MapUsuarioFromReader(reader)));
        }

        public async Task<Usuarios?> GetUsuarioByIdAsync(int id)
        {
            var sql = "SELECT id_user, full_name, user, password, fecha_registro, Usuario_Crea, Equipo_Crea FROM Usuarios WHERE id_user = @ID"; // <--- CAMBIO: ? a @ID para MySQL
            // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma

            return await ExecuteReaderAsync<Usuarios>(sql,
                (command) => command.Parameters.AddWithValue("@ID", id),
                async (reader) => 
                {
                    if (await reader.ReadAsync())
                    {
                        return MapUsuarioFromReader(reader); 
                    }
                    return null;
                });
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }

        public async Task AddUsuariosAsync(Usuarios usuarios)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "INSERT INTO Usuarios (full_name, user, password, fecha_registro, Usuario_Crea, Equipo_Crea) VALUES (@fullName, @user, @password, @fechaRegistro, @usuarioCrea, @equipoCrea)", // <--- CAMBIO: ? a @parametro
                    connection))
                {
                    command.Parameters.AddWithValue("@fullName", usuarios.full_name); 
                    command.Parameters.AddWithValue("@user", usuarios.user);
                    command.Parameters.AddWithValue("@password", usuarios.password);
                    command.Parameters.AddWithValue("@fechaRegistro", usuarios.fecha_registro); 
                    command.Parameters.AddWithValue("@usuarioCrea", usuarios.Usuario_Crea);
                    command.Parameters.AddWithValue("@equipoCrea", usuarios.Equipo_Crea);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task UpdateUsuariosAsync(Usuarios usuarios)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "UPDATE Usuarios SET full_name = @fullName, user = @user, password = @password, fecha_registro = @fechaRegistro, Usuario_Crea = @usuarioCrea, Equipo_Crea = @equipoCrea WHERE id_user = @ID", // <--- CAMBIO: ? a @parametro
                    connection))
                {
                    command.Parameters.AddWithValue("@fullName", usuarios.full_name);
                    command.Parameters.AddWithValue("@user", usuarios.user);
                    command.Parameters.AddWithValue("@password", usuarios.password);
                    command.Parameters.AddWithValue("@fechaRegistro", usuarios.fecha_registro);
                    command.Parameters.AddWithValue("@usuarioCrea", usuarios.Usuario_Crea);
                    command.Parameters.AddWithValue("@equipoCrea", usuarios.Equipo_Crea);
                    command.Parameters.AddWithValue("@ID", usuarios.id_user);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand("DELETE FROM Usuarios WHERE id_user = @ID", connection)) 
                {
                    command.Parameters.AddWithValue("@ID", id);
                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {

            var sql = "SELECT COUNT(*) FROM Usuarios WHERE user = @Username AND password = @Password";

            int count = await ExecuteScalarAsync<int>(sql,
                (command) =>
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password); 
                });

            return count > 0; 
        }
    }
}
