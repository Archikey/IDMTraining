
using Idm.Domain.Entities;

namespace Idm.Application.Manager;


public interface IManagerRole
{
    public Task<Roles?> CreateRoleAsync(Roles role);
    public Task<Roles?> UpdateRolesAsync(Roles role);
    public Task<Roles?> GetRolesByIdAsync(int id);
    public Task<bool> DeleteRolesAsync(int id);
}