
using NpgsqlTypes;
using Npgsql;
using Idm.Domain.Entities;
using Idm.Application.Manager;
using Idm.Infrastructure.ManagerAccount;
using System;
using Idm.Infrastructure.ManagerSystem;
using Dapper;

var connectionString = Environment.GetEnvironmentVariable("IDM_DB_CONNECTION")
?? throw new InvalidOperationException("IDM_DB_CONNECTION environment variable is not set.");
await using var connection = new NpgsqlConnection(connectionString);
await connection.OpenAsync();

IManagerAccount accountManage = new ManagerAccount(connection);
IManagerSystem managerSystem = new ManagerSystem();

try
{
    await managerSystem.AddSystemAsync(connection, "MyLinux", "Linux");
}
catch (PostgresException ex)
    when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
}
var systemId = await managerSystem.SearchSystemAsync(connection, "MyLinux");

var account = new Account
{
    Login = "Archikey",
    DisplayName = "Archikey",
    AccountType = AccountType.Privileged,
    IsActive = true,
    SystemId = systemId,
};

Account? checkAccount;
try
{
    checkAccount = await accountManage.CreateAccountAsync(account);

}
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    Console.WriteLine("An account like that already exists.");
    checkAccount = await accountManage.FindAccountByIdAsync(2);
}

if (checkAccount is not null)
{

    Console.WriteLine($"Display Name: {checkAccount.DisplayName} Id: {checkAccount.Id}");
}



try
{
    Console.WriteLine($"Time {checkAccount!.UpdatedAt}");
    checkAccount!.DisplayName = "Archikey228";
    var result = await accountManage.UpdateAccountAsync(checkAccount!);

    if (result is not null)
    {
        Console.WriteLine($"Time {result!.UpdatedAt}");
        Console.WriteLine($"Display Name: {result.DisplayName} Id: {result.Id}");
    }


    Console.WriteLine("END");
}
catch (System.Exception)
{

    throw;
}


// try
// {
//     checkAccount.Id = 9999;
//     var result = await accountManage.UpdateAccountAsync(checkAccount);
//     if (result is null)
//         System.Console.WriteLine("NULL");



// }
// catch (System.Exception)
// {

//     throw;
// }


string sqlCreateRole = """
INSERT INTO roles (name_role, system_id)
VALUES (@nameRole, @systemId)
RETURNING
id;
""";

string sqlRoleInsert = """
INSERT INTO accounts_roles (accounts_id, roles_id)
VALUES (@accountId, @roleId);
""";

var roleId = await connection.ExecuteScalarAsync<int>(
    sqlCreateRole,
    new
    {
        nameRole = "Admin",
        systemId
    });

await connection.ExecuteAsync(sqlRoleInsert,
new
{
    accountId = checkAccount.Id,
    roleId
}
);

string sqlSelect = """
SELECT COUNT(*)
FROM accounts_roles
WHERE accounts_id = @accountId;
""";
var resultSelect = await connection.ExecuteScalarAsync<long>(sqlSelect,
new
{
    accountId = checkAccount.Id
}
);

System.Console.WriteLine($"result: {resultSelect}");

bool isDeleted = await accountManage.DeleteAccountAsync(checkAccount.Id);
bool isDeleted2 = await accountManage.DeleteAccountAsync(checkAccount.Id);

System.Console.WriteLine($"Test 1 - {isDeleted}\tTest 2 - {isDeleted2}");


resultSelect = await connection.ExecuteScalarAsync<long>(sqlSelect,
new
{
    accountId = checkAccount.Id
}
);

System.Console.WriteLine($"result: {resultSelect}");

