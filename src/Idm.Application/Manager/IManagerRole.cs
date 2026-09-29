
using Idm.Domain.Entities;

namespace Idm.Application.Manager;


public interface IManagerRole
{
    public Task<Role?> CreateRoleAsync(Role role);
    public Task<Role?> UpdateRolesAsync(Role role);
    public Task<Role?> GetRolesByIdAsync(int id);
    public Task<bool> DeleteRolesAsync(int id);
}