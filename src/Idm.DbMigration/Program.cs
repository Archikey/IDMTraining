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

await addedSystem.AddSystemAsync(connection, "Test PostgreSQL", "Training System", "Postgres");
await addedSystem.SearchSystemAsync(connection, "Test PostgreSQL");