using Dapper;
using Npgsql;
using System;
using System.IO;
using System.Linq;
using System.Data.Common;


public class Migration
{
    public async Task Process()
    {
        Console.WriteLine("IDM Database Migrator");

        var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
        ?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        string pathString = @".\database\migrations";

        string checkSqlInitVersion = """
SELECT
    version AS Version,
    applied_at AS AppliedAt
FROM schema_migrations;
""";

        try
        {
            var resultCheck = await connection.QueryAsync<SchemaMigration>(checkSqlInitVersion);


            foreach (var item in resultCheck)
            {
                Console.WriteLine($"{item.Version} | {item.AppliedAt}");
            }
        }
        catch (NpgsqlException ex) when (ex.SqlState == "42P01") //does not exist 
        {
            string numberVersion = "000";


            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await connection.ExecuteAsync(
                    """
            CREATE TABLE IF NOT EXISTS schema_migrations
            (
            version VARCHAR(100) PRIMARY KEY,
            applied_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );
            """,
                    transaction: transaction);

                var newVersion = new SchemaMigration
                {
                    Version = numberVersion,
                    AppliedAt = DateTime.UtcNow
                };

                string sqlInsertVersion = """
            INSERT INTO schema_migrations (version, applied_at)
            VALUES (@Version, @AppliedAt);
            """;

                await connection.ExecuteAsync(
                    sqlInsertVersion,
                    newVersion,
                    transaction: transaction);


                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }


        }


        var versions = FindVersions(pathString);


        var resultCheckLastVersion = await connection.QueryAsync<SchemaMigration>(checkSqlInitVersion);


        var checkSetupVersion = resultCheckLastVersion
        .OrderBy(x => int.Parse(x.Version))
        .Select(x => x.Version)
        .Last();

        List<string> numbersVersion = new List<string>();
        int.TryParse(checkSetupVersion, out int setupVersion);
        foreach (var item in versions)
        {
            var arrItems = item.Split("_");
            int.TryParse(arrItems[0], out int thisVersion);

            if (thisVersion > setupVersion)
                numbersVersion.Add(arrItems[0]);
        }

        foreach (var item in numbersVersion)
        {
            await using var transaction2 = await connection.BeginTransactionAsync();

            try
            {

                var fileSql = SearchFileInitSql(pathString, item);

                var contentSql = await File.ReadAllTextAsync(fileSql);

                await connection.ExecuteAsync(contentSql, transaction: transaction2);


                var newVersion = new SchemaMigration
                {
                    Version = item,
                    AppliedAt = DateTime.UtcNow
                };

                string sqlInsertVersion = """
            INSERT INTO schema_migrations (version, applied_at)
            VALUES (@Version, @AppliedAt);
            """;

                await connection.ExecuteAsync(
                    sqlInsertVersion,
                    newVersion,
                    transaction: transaction2);
                await transaction2.CommitAsync();
            }
            catch (System.Exception)
            {

                await transaction2.RollbackAsync();
                throw;
            }

        }

        System.Console.WriteLine("MIGRATION END");
   
    }
    private string SearchFileInitSql(string path, string number)
    {

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException(
                $"Migration directory not found: {path}");
        }

        string searchPattern = $"{number}_*.sql";
        var files = Directory.GetFiles(path, searchPattern);

        if (files.Length == 0)
        {
            throw new FileNotFoundException(
                $"Migration file for version {number} not found.");
        }

        if (files.Length > 1)
        {
            throw new InvalidOperationException(
                $"Multiple migration files found for version {number}.");
        }

        return files[0];
    }


    private List<string> FindVersions(string pathString)
    {
        return Directory.GetFiles(pathString, "*.sql")
            .OrderBy(x => Path.GetFileName(x).Split('_')[0])
            .Select(x => Path.GetFileName(x)!)
            .ToList();
    }


}
sealed class SchemaMigration
{
    public string Version { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

}



