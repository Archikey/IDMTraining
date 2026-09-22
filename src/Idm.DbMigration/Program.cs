using System.Data.Common;
using ManagerSystem;
using NpgsqlTypes;
using Npgsql;


var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");

await using var connection = new NpgsqlConnection(connectionString);

await connection.OpenAsync();

await using (DbCommand command = connection.CreateCommand())
{

    command.CommandText = @"
    SELECT version();
    ";


    var result = await command.ExecuteScalarAsync();

    Console.WriteLine($"PostgreSQL version: {result}");


}

await using (DbCommand command = connection.CreateCommand())
{
    command.CommandText = @"
    SELECT table_name
    FROM information_schema.tables
    WHERE table_schema = 'public'
    ORDER BY table_name;
    ";

    Console.WriteLine("###############################################");
    await using var reader = await command.ExecuteReaderAsync();
    Console.WriteLine("Tables in the public schema:");

    while (await reader.ReadAsync())
    {
        var tableName = reader.GetString(0);
        Console.WriteLine($"- {tableName}");
    }
}

IAddedSystem addedSystem = new AddedSystem();

await addedSystem.AddSystemAsync(connection, "Test PostgreSQL", "Postgres", "Training System");
await addedSystem.SearchSystemAsync(connection, "Test PostgreSQL");

await addedSystem.AddSystemAsync(connection, "Linux Test", "Linux");
await addedSystem.AddSystemAsync(connection, "Linux Temp", "Linux");
await addedSystem.AddSystemAsync(connection, "Linux Temp 2", "Linux");
await addedSystem.SearchSystemAsync(connection, "Linux Test");



await addedSystem.SearchSystemByIdAsync(connection, 1);
await addedSystem.SearchSystemByIdAsync(connection, 2);
await addedSystem.SearchSystemByIdAsync(connection, 4);


await addedSystem.UpdateSystemTimeAsync(connection, 1, DateTime.UtcNow);
var now = DateTime.Now;
Console.WriteLine($"Value: {now:O}");
Console.WriteLine($"Kind: {now.Kind}");
await addedSystem.UpdateSystemTimeAsync(connection, 1, now);
await addedSystem.SearchSystemByIdAsync(connection, 1);


Console.WriteLine("\n\nUpdating system status to inactive for system ID 1 and 9999...\n\n");

await addedSystem.UpdateSystemStatusAsync(connection, 1, false);
await addedSystem.UpdateSystemStatusAsync(connection, 9999, false);
await addedSystem.SearchSystemByIdAsync(connection, 1);

int id = await addedSystem.SearchSystemAsync(connection, "Linux Temp");

await addedSystem.DeleteSystemAsync(connection, id);
await addedSystem.DeleteSystemAsync(connection, 9999);


id = await addedSystem.SearchSystemAsync(connection, "Linux Temp 2");

await using (var command = connection.CreateCommand())
{
    await Task.Run(async () =>
    {
        string name = "DeleteTestRole";
        string description = "FK test for IDM-005";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@systemId";
        parameter.Value = id;
        parameter.NpgsqlDbType = NpgsqlDbType.Integer;
        command.Parameters.Add(parameter);

        var parameter2 = command.CreateParameter();
        parameter2.ParameterName = "@name";
        parameter2.Value = name;
        parameter2.NpgsqlDbType = NpgsqlDbType.Varchar;
        command.Parameters.Add(parameter2);

        var parameter3 = command.CreateParameter();
        parameter3.ParameterName = "@description";
        parameter3.Value = description;
        parameter3.NpgsqlDbType = NpgsqlDbType.Text;
        command.Parameters.Add(parameter3);




        command.CommandText = @"
        INSERT INTO roles (name_role, description, system_id)
        VALUES (@name, @description, @systemId);
        ";

        await command.ExecuteNonQueryAsync();
    }
    );

    Console.WriteLine($"\n\nInserted a role with a foreign key reference to system ID {id}...\n\n");

    try
    {
        Console.WriteLine($"\n\nAttempting to delete system with ID {id} which has a foreign key reference in the roles table...\n\n");
        
        await addedSystem.DeleteSystemAsync(connection, id);
    }
    catch (NpgsqlException ex) when (ex.SqlState == "23503") // Foreign key violation
    {
        Console.WriteLine($"Foreign key violation occurred: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }




}