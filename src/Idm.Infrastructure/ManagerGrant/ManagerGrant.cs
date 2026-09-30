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
        AND r.id = @roleId;
        """;

        var affectedRows = await _connection.ExecuteAsync(sqlCommand,
        new
        {
            accountId,
            roleId,

        }
        );

        return affectedRows>0;
    }
}
