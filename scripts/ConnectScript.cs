#:package Npgsql@10.0.3
#:project ../src/Idm.Application/Idm.Application.csproj

using NpgsqlTypes;
using Npgsql;
using Idm.Domain.Entities;
using Idm.Application.ManagerAccount;
using System;

var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");
await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

var account = new Account
{
    Login = "Archikey",
    DisplayName = "Archikey",
    AccountType = AccountType.Privileged,
    IsActive = true,
    SystemId = 1,
};
int id = 0;
try
{
    var checkUser = await new AccountManage(connection).CreateAccountAsync(account);

    if (checkUser is not null)
    {
        id = checkUser.Id;
        Console.WriteLine($"Login: {checkUser.Login} Id: {checkUser.Id}");
    }
}
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    Console.WriteLine("An account like that already exists.");
    id = 6;
}


var findAccount = await new AccountManage(connection).FindAccountByIdAsync(id);

if (findAccount is not null)
{
    Console.WriteLine($"Login: {findAccount.Login} Id: {findAccount.Id}");
}