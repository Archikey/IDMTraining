#:package Npgsql@10.0.3
#:package Dapper@2.1.89

using Dapper;
using Npgsql;
using System;
using System.IO;
using System.Linq;
using System.Data.Common;

Console.WriteLine("IDM Database Migrator");

var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");

await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

string pathString = @".\database\migrations";

// string checkSqlInitVersion = """
// SELECT
//     version AS Version,
//     applied_at AS AppliedAt
// FROM schema_migrations;
// """;

// try
// {
//     var resultCheck = await connection.QueryAsync<SchemaMigration>(checkSqlInitVersion);


//     foreach (var item in resultCheck)
//     {
//         Console.WriteLine($"{item.Version} | {item.AppliedAt}");
//     }
// }
// catch (NpgsqlException ex) when (ex.SqlState == "42P01") //does not exist 
// {
//     string numberVersion = "002";
//     var resultSearch = SearchFileInitSql(pathString, numberVersion);


//     string sqlInitExist = await File.ReadAllTextAsync(resultSearch);

//     Console.WriteLine(sqlInitExist);

//     await using var transaction = await connection.BeginTransactionAsync();

//     try
//     {
//         await connection.ExecuteAsync(
//             sqlInitExist,
//             transaction: transaction);

//         var newVersion = new SchemaMigration
//         {
//             Version = numberVersion,
//             AppliedAt = DateTime.UtcNow
//         };

//         string sqlInsertVersion = """
//             INSERT INTO schema_migrations (version, applied_at)
//             VALUES (@Version, @AppliedAt);
//             """;

//         await connection.ExecuteAsync(
//             sqlInsertVersion,
//             newVersion,
//             transaction: transaction);


//         await transaction.CommitAsync();
//     }
//     catch
//     {
//         await transaction.RollbackAsync();
//         throw;
//     }

// }




System.Console.WriteLine(FindMaxVersion(pathString));



static string SearchFileInitSql(string path, string number)
{


    if (!Directory.Exists(path))
    {
        return string.Empty;
    }

    string searchPattern = $"*{number}*.sql";

    string? result = Directory.EnumerateFiles(path, searchPattern).FirstOrDefault();

    return result!;
}


static string FindMaxVersion(string pathString)
{
    var fileNames = Directory.GetFiles(pathString)
                            .Select(Path.GetFileName);

    int max = 0;
    if (fileNames is null)
    {
        throw new ArgumentNullException();
    }
    foreach (var item in fileNames)
    {

        if (item is null)
            throw new ArgumentNullException();

        var valueVersion = item.Split("_");

        int.TryParse(valueVersion[0],out int version);

        if (version <= 0)
        {
            throw new ArgumentException();
        }
        if(max < version)
            max = version;

        
    }

    return max.ToString();
}
sealed class SchemaMigration
{
    public string Version { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

}