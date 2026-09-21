using Npgsql;

namespace ManagerSystem
{
    interface IAddedSystem
    {
        Task AddSystemAsync(NpgsqlConnection npgsqlConnection,
         string newSystemName, string newSystemDescription, string newSystemType);
        Task SearchSystemAsync(NpgsqlConnection npgsqlConnection, string systemName);
    }
}