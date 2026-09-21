using Npgsql;

namespace ManagerSystem
{
    interface IAddedSystem
    {
        Task AddSystemAsync(NpgsqlConnection npgsqlConnection,
         string newSystemName, string newSystemType, string? newSystemDescription = null);
        Task SearchSystemAsync(NpgsqlConnection npgsqlConnection, string systemName);
    }
}