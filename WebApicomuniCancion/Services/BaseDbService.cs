using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using WebApicomuniCancion.Models.Entities;
using Microsoft.Extensions.Configuration; 

namespace WebApicomuniCancion.Services
{
    public abstract class BaseDbService
    {
        private readonly IConfiguration _configuration;

        public BaseDbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection");
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch (MySqlException ex) 
            {
                Console.Error.WriteLine($"Error de base de datos MySQL al abrir la conexión: {ex.Message}");
                connection.Dispose(); 
                throw; 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error inesperado al abrir la conexión MySQL: {ex.Message}");
                connection.Dispose(); 
                throw;
            }
        }

        protected async Task ExecuteNonQueryAsync(Func<MySqlConnection, Task> action)
        {

            using (var connection = await GetOpenConnectionAsync())
            {
                try
                {
                    await action(connection); 
                }
                catch (MySqlException ex) 
                {
                    Console.Error.WriteLine($"Error de base de datos MySQL durante la ejecución de la consulta: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error inesperado durante la ejecución de la consulta MySQL: {ex.Message}");
                    throw;
                }
            }
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
        }


        protected async Task<int> ExecuteNonQueryWithRowsAffectedAsync(Func<MySqlConnection, Task<int>> action)
        {
            using (var connection = await GetOpenConnectionAsync())
            {
                try
                {
                    return await action(connection); 
                }
                catch (MySqlException ex)
                {
                    Console.Error.WriteLine($"Error de base de datos MySQL durante la ejecución de la consulta (con retorno de filas): {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error inesperado durante la ejecución de la consulta MySQL (con retorno de filas): {ex.Message}");
                    throw;
                }
            }
        }

        protected async Task<T> ExecuteReaderAsync<T>(string sql, Action<MySqlCommand> addParameters, Func<MySqlDataReader, Task<T?>> readData) where T : new()
        {
            T? result = default; 


            using (var connection = await GetOpenConnectionAsync()) 
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    addParameters(command); 
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            result = await readData((MySqlDataReader)reader); 
                        }
                    }
                    catch (MySqlException ex) 
                    {
                        Console.Error.WriteLine($"Error de base de datos MySQL durante la lectura: {ex.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error inesperado durante la lectura MySQL: {ex.Message}");
                        throw;
                    }
                }
            }
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            return result;
        }

        protected async Task<List<T>> ExecuteReaderListAsync<T>(string sql, Func<MySqlDataReader, Task<T>> mapRow) where T : class 
        {
            var list = new List<T>();


            using (var connection = await GetOpenConnectionAsync()) 
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(await mapRow((MySqlDataReader)reader)); 
                            }
                        }
                    }
                    catch (MySqlException ex) 
                    {
                        Console.Error.WriteLine($"Error de base de datos MySQL durante la lectura de la lista: {ex.Message}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error inesperado durante la lectura de la lista MySQL: {ex.Message}");
                        throw;
                    }
                }
            }
            // #pragma warning restore CA1416 // Validar la compatibilidad de la plataforma
            return list;
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, Action<MySqlCommand>? addParameters = null)
        {
            try
            {
                using (var connection = await GetOpenConnectionAsync()) 
                {
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        addParameters?.Invoke(command);
                        var result = await command.ExecuteScalarAsync();

                        if (result == DBNull.Value || result == null)
                        {
                            return default(T)!;
                        }

                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.Error.WriteLine($"Error de MySQL en ExecuteScalarAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error inesperado en ExecuteScalarAsync: {ex.Message}");
                throw;
            }
        }
    }
}