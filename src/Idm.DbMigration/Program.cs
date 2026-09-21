using System.Data.Common;
using ManagerSystem;
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
await addedSystem.UpdateSystemStatusAsync(connection, 9999,false);
await addedSystem.SearchSystemByIdAsync(connection, 1);

