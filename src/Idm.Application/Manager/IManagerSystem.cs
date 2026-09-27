using Npgsql;

namespace Idm.Application.Manager
{
    public interface IManagerSystem
    {
        Task AddSystemAsync(NpgsqlConnection npgsqlConnection,
         string newSystemName, string newSystemType, string? newSystemDescription = null);
        Task<int> SearchSystemAsync(NpgsqlConnection npgsqlConnection, string systemName);

        Task SearchSystemByIdAsync(NpgsqlConnection npgsqlConnection, int systemId);

        Task UpdateSystemTimeAsync(NpgsqlConnection npgsqlConnection, int systemId, DateTime newTime);
        Task UpdateSystemStatusAsync(NpgsqlConnection connection, int systemId, bool isActive);

        Task DeleteSystemAsync(NpgsqlConnection connection, int systemId);
    }
}