using NpgsqlTypes;
using Npgsql;
using Idm.Domain.Entities;
using Idm.Application.Manager;
using Idm.Infrastructure.ManagerAccount;
using System;
using Idm.Infrastructure.ManagerSystem;
using Dapper;

public class ProcessRoles
{

    public async Task Start()
    {
        var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
        ?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        IManagerSystem managerSystem = new ManagerSystem(connection);
        IManagerRole managerRole = new ManagerRole(connection);

        try
        {
            await managerSystem.AddSystemAsync("MyLinuxRoles", "Linux");
        }
        catch (PostgresException ex)
            when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
        }
        var systemId = await managerSystem.SearchSystemAsync("MyLinuxRoles");


    }
}