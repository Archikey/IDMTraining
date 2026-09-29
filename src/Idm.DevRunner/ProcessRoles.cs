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

        var newRole = new Role()
        {

            NameRole = "Admin",
            Description = "This test role",
            SystemId = systemId
        };
        var resultRole = await managerRole.CreateRoleAsync(newRole);

        Console.WriteLine($"ID: {resultRole!.Id}\tName: {resultRole!.NameRole}\tSystemID: {resultRole!.SystemId}\tDescription: {resultRole!.Description}");

        try
        {
            await managerRole.CreateRoleAsync(new Role
            {
                NameRole = "Admin",
                Description = "This test role",
                SystemId = systemId
            });
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }

        try
        {
            await managerRole.CreateRoleAsync(new Role
            {
                NameRole = "!qwwAdmin",
                Description = "This test role",
                SystemId = 999999
            });
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }

        var getRole = await managerRole.GetRolesByIdAsync(resultRole.Id);

        Console.WriteLine($"ID: {getRole!.Id}\tName: {getRole!.NameRole}\tSystemID: {getRole!.SystemId}");

        getRole = null;

        resultRole.NameRole = "Helper";

        getRole = await managerRole.UpdateRolesAsync(resultRole);

        Console.WriteLine($"ID: {getRole!.Id}\tName: {getRole!.NameRole}\tSystemID: {getRole!.SystemId}");

        bool isDeleted = await managerRole.DeleteRolesAsync(resultRole.Id);
        System.Console.WriteLine($"IsDeleted = {isDeleted}");
        getRole = await managerRole.GetRolesByIdAsync(resultRole.Id);

        if (getRole is null)
        {
            System.Console.WriteLine("GetRole is NULL");
        }

        isDeleted = await managerRole.DeleteRolesAsync(resultRole.Id);
        System.Console.WriteLine($"IsDeleted = {isDeleted}");

    }
}