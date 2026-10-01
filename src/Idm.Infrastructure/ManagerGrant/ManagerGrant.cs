using Idm.Domain.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;
using Idm.Application.Manager;
using Dapper;


namespace Idm.Infrastructure.ManagerGrant;

public class ManagerGrant(NpgsqlConnection npgsqlConnection) : IManagerGrant
{
    private readonly NpgsqlConnection _connection = npgsqlConnection;



    public async Task<bool> GrantRoleAsync(int accountId, int roleId)
    {


        string sqlCommand = """
        INSERT INTO accounts_roles (accounts_id, roles_id)
        SELECT
        a.id,
        r.id
        FROM accounts AS a
        JOIN roles AS r
        ON a.system_id = r.system_id
        WHERE a.id = @accountId
        AND r.id = @roleId
        ON CONFLICT (accounts_id, roles_id) DO NOTHING;
        """;

        await _connection.ExecuteAsync(
          sqlCommand,
          new { accountId, roleId });

        string selectRow = """
            SELECT EXISTS (
            SELECT 1
            FROM accounts_roles
            WHERE accounts_id = @accountId
            AND roles_id = @roleId
            );
        """;

        bool exists = await _connection.ExecuteScalarAsync<bool>(
            selectRow,
            new { accountId, roleId });

        return exists;
    }

    public async Task<bool> RevokeRoleAsync(int accountId, int roleId)
    {
        string sqlCommand = """
        DELETE FROM accounts_roles
        WHERE accounts_id = @accountId
        AND roles_id = @roleId;
        """;

        await _connection.ExecuteAsync(sqlCommand, new { accountId, roleId });

        string selectRow = """
            SELECT EXISTS (
            SELECT 1
            FROM accounts_roles
            WHERE accounts_id = @accountId
            AND roles_id = @roleId
            );
        """;

        bool exists = await _connection.ExecuteScalarAsync<bool>(
            selectRow,
            new { accountId, roleId });

        return !exists;
    }
    public async Task<List<Role>> GetAccountRolesAsync(int accountId)
    {
        string sqlCommand = """
        SELECT
         r.id AS Id,
         r.name_role AS NameRole,
         r.description AS Description,
         r.created_at AS CreatedAt,
         r.system_id AS SystemId
        FROM roles AS r
        JOIN accounts_roles AS ar ON r.id = ar.roles_id
        WHERE ar.accounts_id = @accountId;
        """;

        var roles = await _connection.QueryAsync<Role>(
    sqlCommand,
    new { accountId });

        return roles.ToList();
    }

    public async Task<List<Account>> GetRoleAccountsAsync(int roleId)
    {
        var sqlCommand = """
        SELECT
         a.id AS Id,
         a.login_account AS Login,
         a.display_name AS DisplayName,
         a.account_type AS AccountType,
         a.is_active AS IsActive,
         a.system_id AS SystemId,
         a.created_at AS CreatedAt,
         a.updated_at AS UpdatedAt
        FROM accounts AS a
        JOIN accounts_roles AS ar ON a.id = ar.accounts_id
        WHERE ar.roles_id = @roleId;
        """;

        var accounts = await _connection.QueryAsync<Account>(
            sqlCommand,
            new { roleId });

        return accounts.ToList();
    }
}
