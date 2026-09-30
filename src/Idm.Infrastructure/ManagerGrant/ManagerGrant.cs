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
}
