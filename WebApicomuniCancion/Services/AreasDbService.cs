using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient; 
using System.Threading.Tasks;
using WebApicomuniCancion.Models.Entities;
using Microsoft.Extensions.Configuration; 

namespace WebApicomuniCancion.Services
{
    public class AreasDbService : BaseDbService, IAreasDbService
    {
        public AreasDbService(IConfiguration configuration) : base(configuration)
        {
        }

        private Area MapAreaFromReader(MySqlDataReader reader) 
        {
            return new Area
            {
                Id_Area = reader.GetInt32(reader.GetOrdinal("Id_Area")),
                Area_Desarrollo = reader.GetString(reader.GetOrdinal("Area_Desarrollo")),
                Fecha_Registro = reader.IsDBNull(reader.GetOrdinal("Fecha_Registro"))
                                            ? (DateTime?)null
                                            : (DateTime?)reader.GetValue(reader.GetOrdinal("Fecha_Registro")),
                Descipcion_Area = reader.IsDBNull(reader.GetOrdinal("Descipcion_Area"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Descipcion_Area")),
                Usuario_Crea = reader.IsDBNull(reader.GetOrdinal("Usuario_Crea"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Usuario_Crea")),
                Equipo_Crea = reader.IsDBNull(reader.GetOrdinal("Equipo_Crea"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Equipo_Crea"))
            };
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }

        public async Task<List<Area>> GetAllAreasAsync()
        {
            var sql = "SELECT Id_Area, Area_Desarrollo, Fecha_Registro, Descipcion_Area, Usuario_Crea, Equipo_Crea FROM Areas";

            return await ExecuteReaderListAsync(sql, async (reader) => await Task.FromResult(MapAreaFromReader(reader)));
        }

        public async Task<Area?> GetAreaByIdAsync(int id)
        {
            var sql = "SELECT Id_Area, Area_Desarrollo, Fecha_Registro, Descipcion_Area, Usuario_Crea, Equipo_Crea FROM Areas WHERE Id_Area = @ID"; 
            // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma

            return await ExecuteReaderAsync<Area>(sql,
                (command) => command.Parameters.AddWithValue("@ID", id),
                async (reader) => 
                {
                    if (await reader.ReadAsync())
                    {
                        return MapAreaFromReader(reader); 
                    }
                    return null;
                });
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }

        public async Task AddAreaAsync(Area area)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "INSERT INTO Areas (Area_Desarrollo, Fecha_Registro, Descipcion_Area, Usuario_Crea, Equipo_Crea) VALUES (@areaDesarrollo, @fechaRegistro, @descripcionArea, @usuarioCrea, @equipoCrea)", 
                    connection))
                {
                    command.Parameters.AddWithValue("@areaDesarrollo", area.Area_Desarrollo); 
                    command.Parameters.AddWithValue("@fechaRegistro", area.Fecha_Registro); 
                    command.Parameters.AddWithValue("@descripcionArea", area.Descipcion_Area);
                    command.Parameters.AddWithValue("@usuarioCrea", area.Usuario_Crea);
                    command.Parameters.AddWithValue("@equipoCrea", area.Equipo_Crea);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task UpdateAreaAsync(Area area)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "UPDATE Areas SET Area_Desarrollo = @areaDesarrollo, Fecha_Registro = @fechaRegistro, Descipcion_Area = @descripcionArea, Usuario_Crea = @usuarioCrea, Equipo_Crea = @equipoCrea WHERE Id_Area = @ID", 
                    connection))
                {
                    command.Parameters.AddWithValue("@areaDesarrollo", area.Area_Desarrollo);
                    command.Parameters.AddWithValue("@fechaRegistro", area.Fecha_Registro);
                    command.Parameters.AddWithValue("@descripcionArea", area.Descipcion_Area);
                    command.Parameters.AddWithValue("@usuarioCrea", area.Usuario_Crea);
                    command.Parameters.AddWithValue("@equipoCrea", area.Equipo_Crea);
                    command.Parameters.AddWithValue("@ID", area.Id_Area);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task DeleteAreaAsync(int id)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand("DELETE FROM Areas WHERE Id_Area = @ID", connection)) 
                {
                    command.Parameters.AddWithValue("@ID", id);
                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }
    }
}