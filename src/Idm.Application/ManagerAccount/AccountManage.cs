using Idm.Domain.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;

namespace Idm.Application.ManagerAccount;

public class AccountManage(NpgsqlConnection npgsqlConnection) : IAccountManage
{

    private readonly NpgsqlConnection _connection = npgsqlConnection;

    public async Task<Account?> CreateAccountAsync(Account account)
    {
        await using var command = _connection.CreateCommand();

        //FactoryParameter.CreateAndAddParameter(command,"@accountId",FactoryType.Integer,account.Id);
        FactoryParameter.CreateAndAddParameter(command, "@login", FactoryType.Varchar, account.Login);
        FactoryParameter.CreateAndAddParameter(command, "@displayName", FactoryType.Varchar, account.DisplayName);
        FactoryParameter.CreateAndAddParameter(command, "@accountType", FactoryType.Varchar, account.AccountType.ToString());
        FactoryParameter.CreateAndAddParameter(command, "@isActive", FactoryType.Boolean, account.IsActive);
        FactoryParameter.CreateAndAddParameter(command, "@systemId", FactoryType.Integer, account.SystemId);
        FactoryParameter.CreateAndAddParameter(command, "@createdAt", FactoryType.DateTime, account.CreatedAt);
        FactoryParameter.CreateAndAddParameter(command, "@updatedAt", FactoryType.DateTime, account.UpdatedAt);

        command.CommandText = """
        INSERT INTO accounts 
        (login_account, account_type, display_name, created_at, updated_at, is_active, system_id)
        VALUES
        (@login, @accountType, @displayName, @createdAt, @updatedAt, @isActive, @systemId)
        RETURNING
        id,
        login_account,
        account_type,
        display_name,
        created_at,
        updated_at,
        is_active,
        system_id;
        """;

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var createdAccount = MapAccount(reader);

            return createdAccount;
        }
        else
        {
            return null;
        }
    }


    public async Task<List<Account>> FindAccountByBelongingToSystemAsync(int systemId)
    {
        await using var command = _connection.CreateCommand();
        FactoryParameter.CreateAndAddParameter(command, "@systemId", FactoryType.Integer, systemId);

        command.CommandText = """
        SELECT
        a.id,
        a.login_account,
        a.account_type,
        a.display_name,
        a.created_at,
        a.updated_at,
        a.is_active,
        a.system_id
        FROM accounts AS a
        INNER JOIN systems AS s 
        ON a.system_id = s.id
        WHERE a.system_id = @systemId
        AND
        s.is_active = true
        """;

        await using var reader = await command.ExecuteReaderAsync();
        List<Account> accounts = new List<Account>();
        while (await reader.ReadAsync())
        {
            var acc = MapAccount(reader);
            accounts.Add(acc);
        }

        return accounts;
    }

    public async Task<Account?> FindAccountByIdAsync(int id)
    {
        await using var command = _connection.CreateCommand();
        FactoryParameter.CreateAndAddParameter(command, "@Id", FactoryType.Integer, id);

        command.CommandText = """
        SELECT * FROM accounts
        WHERE id = @id;
        """;

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var createdAccount = MapAccount(reader);

            return createdAccount;
        }
        else
        {
            return null;
        }
    }

    private static Account MapAccount(NpgsqlDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(0),
            Login = reader.GetString(1),
            AccountType = Enum.Parse<AccountType>(reader.GetString(2)),
            DisplayName = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4),
            UpdatedAt = reader.GetDateTime(5),
            IsActive = reader.GetBoolean(6),
            SystemId = reader.GetInt32(7)
        };
}