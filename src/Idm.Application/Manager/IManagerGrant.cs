using Idm.Domain.Entities;

namespace Idm.Application.Manager;

public interface IManagerGrant
{
    public Task<bool> GrantRoleAsync(int accountId, int roleId);
}