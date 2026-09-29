using Npgsql;

namespace Idm.Application.Manager
{
    public interface IManagerSystem
    {
        Task AddSystemAsync(string newSystemName, string newSystemType, string? newSystemDescription = null);
        Task<int> SearchSystemAsync(string systemName);

        Task SearchSystemByIdAsync(int systemId);

        Task UpdateSystemTimeAsync(int systemId, DateTime newTime);
        Task UpdateSystemStatusAsync(int systemId, bool isActive);

        Task DeleteSystemAsync(int systemId);
    }
}