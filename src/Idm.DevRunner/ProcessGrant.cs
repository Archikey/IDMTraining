using NpgsqlTypes;
using Npgsql;
using Idm.Domain.Entities;
using Idm.Application.Manager;
using Idm.Infrastructure.ManagerAccount;
using System;
using Idm.Infrastructure.ManagerSystem;
using Dapper;
using Idm.Infrastructure.ManagerGrant;


public class ProcessGrant
{
    public async Task Start()
    {
        var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
        ?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        IManagerSystem managerSystem = new ManagerSystem(connection);
        IManagerRole managerRole = new ManagerRole(connection);
        IManagerAccount managerAccount = new ManagerAccount(connection);
        IManagerGrant managerGrant = new ManagerGrant(connection);

        try
        {
            await managerSystem.AddSystemAsync("MyLinuxGrant", "Linux");
        }
        catch (PostgresException ex)
            when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
        }
        var systemId = await managerSystem.SearchSystemAsync("MyLinuxGrant");


        // var account = new Account
        // {
        //     Login = "ArchikeyGrant2",
        //     DisplayName = "ArchikeyGrant",
        //     AccountType = AccountType.Privileged,
        //     IsActive = true,
        //     SystemId = systemId,
        // };
        // Account? checkAccount;
        // int accId =0;
        // try
        // {
        //     checkAccount = await managerAccount.CreateAccountAsync(account);
        //     accId = checkAccount!.Id;
        //     //14 acc
        // }
        // catch (Exception)
        // {
        //     Console.WriteLine("An account like that already exists.");
        // }
        // var newRole = new Role()
        // {

        //     NameRole = "Admin2",
        //     Description = "This test role",
        //     SystemId = systemId
        // };
        // var resultRole = await managerRole.CreateRoleAsync(newRole);



        // bool resultDead = await managerGrant.GrantRoleAsync(13,6);
        // System.Console.WriteLine(resultDead);

        bool result = await managerGrant.GrantRoleAsync(14, 6);
        // System.Console.WriteLine(result);

        result = await managerGrant.RevokeRoleAsync(14, 6);
        System.Console.WriteLine(result);

        result = await managerGrant.RevokeRoleAsync(14, 6);
        System.Console.WriteLine(result);

        result = await managerGrant.GrantRoleAsync(14, 6);
        System.Console.WriteLine(result);

        // bool resultNew = await managerGrant.GrantRoleAsync(accId,resultRole!.Id);
        // System.Console.WriteLine(resultNew);

    }
}