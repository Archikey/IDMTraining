using System.Data.Common;
using Npgsql;


var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION") 
?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");

await using var connection = new NpgsqlConnection(connectionString);

await connection.OpenAsync();

DbCommand command = connection.CreateCommand();

command.CommandText = @"
SELECT version();
";


var result = await command.ExecuteScalarAsync();

Console.WriteLine($"PostgreSQL version: {result}");