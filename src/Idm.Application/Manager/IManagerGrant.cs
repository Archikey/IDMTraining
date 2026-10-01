using Idm.Domain.Entities;

namespace Idm.Application.Manager;

public interface IManagerGrant
{
    public Task<bool> GrantRoleAsync(int accountId, int roleId);
    public Task<bool> RevokeRoleAsync(int accountId, int roleId);

    public Task<List<Role>> GetAccountRolesAsync(int accountId);
    public Task<List<Account>> GetRoleAccountsAsync(int roleId);
    public Task<bool> HasRoleAsync(int accountId, int roleId);
    public Task<int> GetAccountRoleCountAsync(int accountId);
}