using NpgsqlTypes;
using Npgsql;

namespace ManagerSystem
{
    public class AddedSystem : IAddedSystem
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

        public async Task SearchSystemAsync(NpgsqlConnection npgsqlConnection, string systemName)
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
                }
                else
                {
                    Console.WriteLine($"No system found with name: {systemName}");
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
                    SELECT id, name_system, is_active, created_at
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

                    Console.WriteLine($"Found system: ID={id}, Name={name}, Is Active={isActive}, Created At={createdAt}, Created At Type={createdAtType}");

                    Console.WriteLine($"Kind: {createdAt.Kind}");
                    Console.WriteLine($"UTC: {createdAt:O}");
              
                }
                else
                {
                    Console.WriteLine($"No system found with ID: {systemId}");
                }
            }
        }
    }
}