#:package Npgsql@10.0.3
#:project ../src/Idm.Application/Idm.Application.csproj
#:project ../src/Idm.Infrastructure/Idm.Infrastructure.csproj

using NpgsqlTypes;
using Npgsql;
using Idm.Domain.Entities;
using Idm.Application.Manager;
using Idm.Infrastructure.ManagerAccount;
using System;
using Idm.Infrastructure.ManagerSystem;

var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");
await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

IAccountManage accountManage = new AccountManage(connection);
IManagerSystem managerSystem = new ManagerSystem();

try
{
    await managerSystem.AddSystemAsync(connection, "MyLinux", "Linux");
}
catch
{
    
}

var account = new Account
{
    Login = "Archikey",
    DisplayName = "Archikey",
    AccountType = AccountType.Privileged,
    IsActive = true,
    SystemId = 0,
};

Account? checkAccount;
try
{
    checkAccount = await accountManage.CreateAccountAsync(account);

    if (checkAccount is not null)
    {
        
        Console.WriteLine($"Login: {checkAccount.Login} Id: {checkAccount.Id}");
    }
}
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    Console.WriteLine("An account like that already exists.");
    checkAccount = await accountManage.FindAccountByIdAsync(1);
}

account.DisplayName= "Archikey228";

var result = await accountManage.UpdateAccountAsync(account);

if (result is not null)
    Console.WriteLine($"Login: {result.DisplayName} Id: {result.Id}");

Console.WriteLine("END");