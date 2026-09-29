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
    public Task<Roles> CreateRoleAsync(Roles role, int systemId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRolesAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Roles> GetRolesByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Roles> UpdateRolesAsync(Roles role)
    {
        throw new NotImplementedException();
    }
}