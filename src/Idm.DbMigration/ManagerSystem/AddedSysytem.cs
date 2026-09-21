
using Npgsql;

namespace ManagerSystem
{
    public class AddedSystem : IAddedSystem
    {
        public async Task AddSystemAsync(NpgsqlConnection npgsqlConnection,
         string newSystemName, string newSystemDescription, string newSystemType)
        {


            await using (var command = npgsqlConnection.CreateCommand())
            {
              
                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "@newSystemName";
                nameParameter.Value = newSystemName;
                command.Parameters.Add(nameParameter);
                
                var descriptionParameter = command.CreateParameter();
                descriptionParameter.ParameterName = "@newSystemDescription";
                descriptionParameter.Value = newSystemDescription;
                command.Parameters.Add(descriptionParameter);

                var typeParameter = command.CreateParameter();
                typeParameter.ParameterName = "@newSystemType";
                typeParameter.Value = newSystemType;
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
                    var description = reader.GetString(2);
                    var systemType = reader.GetString(3);

                    Console.WriteLine($"Found system: ID={id}, Name={name}, Description={description}, Type={systemType}");
                }
                else
                {
                    Console.WriteLine($"No system found with name: {systemName}");
                }
            }
        }
    }
}