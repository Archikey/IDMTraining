using NpgsqlTypes;
using System;
using Npgsql;
using Idm.Application.Manager;
using Idm.Infrastructure.ManagerAccount;

namespace Idm.Infrastructure.ManagerSystem;

    public class ManagerSystem : IManagerSystem
    {
        public async Task AddSystemAsync(NpgsqlConnection npgsqlConnection,
         string newSystemName, string newSystemType, string? newSystemDescription = null)
        {


            await using (var command = npgsqlConnection.CreateCommand())
            {

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "@newSystemName";
                nameParameter.Value = newSystemName;
                nameParameter.NpgsqlDbType = NpgsqlDbType.Varchar;
                command.Parameters.Add(nameParameter);

                var descriptionParameter = command.CreateParameter();
                descriptionParameter.ParameterName = "@newSystemDescription";
                descriptionParameter.NpgsqlDbType = NpgsqlDbType.Text;
                if (newSystemDescription is null)
                {

                    descriptionParameter.Value = DBNull.Value;

                }
                else
                {
                    descriptionParameter.Value = newSystemDescription;
                }
                command.Parameters.Add(descriptionParameter);



                var typeParameter = command.CreateParameter();
                typeParameter.ParameterName = "@newSystemType";
                typeParameter.Value = newSystemType;
                typeParameter.NpgsqlDbType = NpgsqlDbType.Varchar;
                command.Parameters.Add(typeParameter);

                command.CommandText = @"
                    INSERT INTO systems
                    (name_system, description, system_type)
                    VALUES
                    (@newSystemName, @newSystemDescription, @newSystemType)
                ";

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> SearchSystemAsync(NpgsqlConnection npgsqlConnection, string systemName)
        {
            await using (var command = npgsqlConnection.CreateCommand())
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@systemName";
                parameter.Value = systemName;
                command.Parameters.Add(parameter);

                command.CommandText = @"
                    SELECT id, name_system, description, system_type
                    FROM systems
                    WHERE name_system = @systemName
                ";

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);



                    string? description = reader.IsDBNull(2) ? null : reader.GetString(2);

                    var systemType = reader.GetString(3);

                    Console.WriteLine($"Found system: ID={id}, Name={name}, Description={(description is null ? "null" : description)}, Type={systemType}");
                    return id;
                }
                else
                {
                    Console.WriteLine($"No system found with name: {systemName}");
                    return -1;
                }
            }
        }

        public async Task SearchSystemByIdAsync(NpgsqlConnection npgsqlConnection, int systemId)
        {
            await using (var command = npgsqlConnection.CreateCommand())
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@systemId";
                parameter.Value = systemId;
                parameter.NpgsqlDbType = NpgsqlDbType.Integer;
                command.Parameters.Add(parameter);

                command.CommandText = @"
                    SELECT id, name_system, is_active, created_at,
                    updated_at
                    FROM systems
                    WHERE id = @systemId
                ";

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    var isActive = reader.GetBoolean(2);
                    var createdAt = reader.GetDateTime(3);
                    var createdAtType = reader.GetFieldType(3);
                    var updatedAt = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

                    Console.WriteLine($"Found system: ID={id}, Name={name}, Is Active={isActive}, Created At={createdAt}, Created At Type={createdAtType}, Updated At={(updatedAt is null ? "null" : updatedAt.ToString())}");

                    Console.WriteLine($"Kind: {createdAt.Kind}");
                    Console.WriteLine($"UTC: {createdAt:O}");
              
                }
                else
                {
                    Console.WriteLine($"No system found with ID: {systemId}");
                }
            }
        }

        public async Task UpdateSystemTimeAsync(NpgsqlConnection npgsqlConnection, int systemId, DateTime newTime)
        {
            await using (var command = npgsqlConnection.CreateCommand())
            {

                DateTime utcTime = newTime.ToUniversalTime();

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@systemId";
                idParameter.Value = systemId;
                idParameter.NpgsqlDbType = NpgsqlDbType.Integer;
                command.Parameters.Add(idParameter);

                var timeParameter = command.CreateParameter();
                timeParameter.ParameterName = "@newTime";
                timeParameter.Value = utcTime;
                timeParameter.NpgsqlDbType = NpgsqlDbType.TimestampTz;
                command.Parameters.Add(timeParameter);

                command.CommandText = @"
                    UPDATE systems
                    SET created_at = @newTime
                    WHERE id = @systemId
                ";

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateSystemStatusAsync(NpgsqlConnection connection, int systemId, bool isActive)
        {
            await using (var command = connection.CreateCommand())
            {
                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@systemId";
                idParameter.Value = systemId;
                idParameter.NpgsqlDbType = NpgsqlDbType.Integer;
                command.Parameters.Add(idParameter);

                var isActiveParameter = command.CreateParameter();
                isActiveParameter.ParameterName = "@isActive";
                isActiveParameter.Value = isActive;
                isActiveParameter.NpgsqlDbType = NpgsqlDbType.Boolean;
                command.Parameters.Add(isActiveParameter);

                DateTime currentTime = DateTime.UtcNow;
                var timeParameter = command.CreateParameter();
                timeParameter.ParameterName = "@currentTime";
                timeParameter.Value = currentTime;
                timeParameter.NpgsqlDbType = NpgsqlDbType.TimestampTz;
                command.Parameters.Add(timeParameter);

                command.CommandText= @"
                    UPDATE systems
                    SET is_active = @isActive, 
                    updated_at = @currentTime
                    WHERE id = @systemId
                    ";

              var result = await command.ExecuteNonQueryAsync();

           

              if (result == 0)
              {
                  Console.WriteLine($"No system found with ID: {systemId}. No rows updated.");
              } 
              else
              {
                  Console.WriteLine($"System ID {systemId} updated successfully.\t");
                  Console.Write("{0} row(s) updated.\n", result);
              }
            }
            
        }

        public async Task DeleteSystemAsync(NpgsqlConnection connection, int systemId)
        {
            await using (var command = connection.CreateCommand())
            {
                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@systemId";
                idParameter.Value = systemId;
                idParameter.NpgsqlDbType = NpgsqlDbType.Integer;
                command.Parameters.Add(idParameter);

                command.CommandText = @"
                    DELETE FROM systems
                    WHERE id = @systemId";

               var result = await command.ExecuteNonQueryAsync();

                if (result == 0)
                {
                    Console.WriteLine($"No system found with ID: {systemId}. No rows deleted.");
                }
                else
                {
                    Console.WriteLine($"System ID {systemId} deleted successfully.\t");
                    Console.Write("{0} row(s) deleted.\n", result);
                }
            }       
        }
    }
