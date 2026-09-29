#:package Npgsql@10.0.3
#:project ../src/Idm.Application/Idm.Application.csproj
#:project ../src/Idm.Infrastructure/Idm.Infrastructure.csproj

using NpgsqlTypes;
using Npgsql;
using Idm.Domain.Entities;
using Idm.Application.Manager;
using Idm.Infrastructure.ManagerAccount;
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
    SystemId = 9,
};
int id = 0;
try
{
    var checkUser = await new ManagerAccount(connection).CreateAccountAsync(account);

    if (checkUser is not null)
    {
        id = checkUser.Id;
        Console.WriteLine($"Login: {checkUser.Login} Id: {checkUser.Id}");
    }
}
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    Console.WriteLine("An account like that already exists.");
    id = 19;
}


var findAccount = await new ManagerAccount(connection).FindAccountByIdAsync(id);

if (findAccount is not null)
{
    Console.WriteLine($"Find - Login: {findAccount.Login} Id: {findAccount.Id}");
}



var account2 = new Account
{
    Login = "Archikey228",
    DisplayName = "Archikey228",
    AccountType = AccountType.Privileged,
    IsActive = true,
    SystemId = 9,
};
try
{
    var checkUser = await new ManagerAccount(connection).CreateAccountAsync(account2);

    if (checkUser is not null)
    {
        id = checkUser.Id;
        Console.WriteLine($"Login: {checkUser.Login} Id: {checkUser.Id}");
    }
}
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    Console.WriteLine("An account like that already exists.");
}


var accounts = await new ManagerAccount(connection).FindAccountByBelongingToSystemAsync(9);

foreach (var item in accounts)
{
    Console.WriteLine($"Login: {item.Login} - Id: {item.Id}");
}

var accountStatistic = await new ManagerAccount(connection).GetAccountStatisticsAsync();

foreach (var item in accountStatistic)
{
    Console.WriteLine($"System Name: {item.SystemName}\tAccount type: {item.AccountType}\tCount: {item.Count}");
}
