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
        throw new NotImplementedException();
    }

    public async Task<List<Account>> FindAccountByBelongingToSystemAsync(int systemId)
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> FindAccountByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}