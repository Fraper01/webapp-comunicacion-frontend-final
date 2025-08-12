using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient; 
using System.Threading.Tasks;
using WebApicomuniCancion.Interfaces;
using WebApicomuniCancion.Models.Entities;
using Microsoft.Extensions.Configuration;

namespace WebApicomuniCancion.Services
{
    public class AreasAttDbService : BaseDbService, IAreasAttDbService
    {
        public AreasAttDbService(IConfiguration configuration) : base(configuration)
        {
        }

        private Areas_Att MapAreaAttFromReader(MySqlDataReader reader) 
        {
            return new Areas_Att
            {
                Id_AreaAtt = reader.GetInt32(reader.GetOrdinal("Id_AreaAtt")),
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
        }

        public async Task<List<Areas_Att>> GetAllAreasAttAsync()
        {
            var sql = "SELECT Id_AreaAtt, Area_Desarrollo, Fecha_Registro, Descipcion_Area, Usuario_Crea, Equipo_Crea FROM Areas_Atte";

            return await ExecuteReaderListAsync(sql, async (reader) => await Task.FromResult(MapAreaAttFromReader(reader)));
        }

        public async Task<Areas_Att?> GetAreaAttByIdAsync(int id)
        {
            var sql = "SELECT Id_AreaAtt, Area_Desarrollo, Fecha_Registro, Descipcion_Area, Usuario_Crea, Equipo_Crea FROM Areas_Atte WHERE Id_AreaAtt = @ID"; // <--- CAMBIO: ? a @ID para MySQL
            // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma

            return await ExecuteReaderAsync<Areas_Att>(sql,
                (command) => command.Parameters.AddWithValue("@ID", id),
                async (reader) => 
                {
                    if (await reader.ReadAsync())
                    {
                        return MapAreaAttFromReader(reader); 
                    }
                    return null;
                });
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }

        public async Task AddAreaAttAsync(Areas_Att areas_att)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "INSERT INTO Areas_Atte (Area_Desarrollo, Fecha_Registro, Descipcion_Area, Usuario_Crea, Equipo_Crea) VALUES (@areaDesarrollo, @fechaRegistro, @descripcionArea, @usuarioCrea, @equipoCrea)", // <--- CAMBIO: ? a @parametro
                    connection))
                {
                    command.Parameters.AddWithValue("@areaDesarrollo", areas_att.Area_Desarrollo); 
                    command.Parameters.AddWithValue("@fechaRegistro", areas_att.Fecha_Registro); 
                    command.Parameters.AddWithValue("@descripcionArea", areas_att.Descipcion_Area);
                    command.Parameters.AddWithValue("@usuarioCrea", areas_att.Usuario_Crea);
                    command.Parameters.AddWithValue("@equipoCrea", areas_att.Equipo_Crea);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task UpdateAreaAttAsync(Areas_Att area_att)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand( 
                    "UPDATE Areas_Atte SET Area_Desarrollo = @areaDesarrollo, Fecha_Registro = @fechaRegistro, Descipcion_Area = @descripcionArea, Usuario_Crea = @usuarioCrea, Equipo_Crea = @equipoCrea WHERE Id_AreaAtt = @ID", // <--- CAMBIO: ? a @parametro
                    connection))
                {
                    command.Parameters.AddWithValue("@areaDesarrollo", area_att.Area_Desarrollo);
                    command.Parameters.AddWithValue("@fechaRegistro", area_att.Fecha_Registro);
                    command.Parameters.AddWithValue("@descripcionArea", area_att.Descipcion_Area);
                    command.Parameters.AddWithValue("@usuarioCrea", area_att.Usuario_Crea);
                    command.Parameters.AddWithValue("@equipoCrea", area_att.Equipo_Crea);
                    command.Parameters.AddWithValue("@ID", area_att.Id_AreaAtt);

                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }

        public async Task DeleteAreaAttAsync(int id)
        {
            await ExecuteNonQueryAsync(async (connection) =>
            {
                // #pragma warning disable CA1416 // Validar la compatibilidad de la plataforma
                using (var command = new MySqlCommand("DELETE FROM Areas_Atte WHERE Id_AreaAtt = @ID", connection)) 
                {
                    command.Parameters.AddWithValue("@ID", id);
                    await command.ExecuteNonQueryAsync();
                }
                // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            });
        }
    }
}