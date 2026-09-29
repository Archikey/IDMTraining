using Idm.Domain.Entities;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Data.Common;
using Idm.Application.Manager;
using Dapper;

namespace Idm.Infrastructure.ManagerAccount;

public class ManagerRole(NpgsqlConnection npgsqlConnection) : IManagerRole
{
    private readonly NpgsqlConnection _connection = npgsqlConnection;
    public async Task<Roles?> CreateRoleAsync(Roles role)
    {
        string sqlCreate = """
        INSERT INTO roles (name_role,description, system_id)
        VALUES
        (@NameRole, @Description, @SystemId)
        RETURNING
        id AS Id,
        name_role AS NameRole,
        description AS Description
        created_at AS CreatedAt,
        system_id AS SystemId;
        """;
        var result = await _connection.QuerySingleAsync<Roles>(sqlCreate,
        new
        {
            role.NameRole,
            role.Description,
            role.SystemId
        }
        );

        return result;
    }

    public async Task<bool> DeleteRolesAsync(int id)
    {
        string sqlDelete = """
        DELETE FROM roles
        WHERE id = @id
        """;

        var result = await _connection.ExecuteAsync(sqlDelete, new { id });

        return result > 0;
    }

    public async Task<Roles?> GetRolesByIdAsync(int id)
    {
        string sqlGet = """
        SELECT 
        id AS Id,
        name_role AS NameRole,
        description AS Description,
        created_at AS CreatedAt,
        system_id AS SystemId
        FROM roles
        WHERE id = @id;
        """;

        var result = await _connection.QuerySingleOrDefaultAsync<Roles>(sqlGet, new { id });
        return result;
    }

    public async Task<Roles?> UpdateRolesAsync(Roles role)
    {
        string sqlUpdate = """
        UPDATE roles
        SET name_role = @NameRole, description = @Description, system_id = @SystemId
        WHERE id = @Id
        RETURNING
        id AS Id,
        name_role AS NameRole,
        description AS Description,
        created_at AS CreatedAt,
        system_id AS SystemId;
        """;

        var result = await _connection.QuerySingleOrDefaultAsync<Roles>(sqlUpdate,
        new
        {
            role.Id,
            role.NameRole,
            role.Description,
            role.SystemId
        }
        );

        return result;
    }
}