using MongoDB.Driver;

namespace MongoDBRepository.Interfaces;

public interface IMongoContext : IDisposable
{
    string DefaultDatabaseName { get; set; }
    string? RoleToDefaultDatabase { get; set; }
    bool IsTenantEnabled { get; set; }
    IMongoDatabase Database { get; set; }
    void AddCommand(Func<Task> func);
    Task<int> SaveChangesAsync();
    IMongoCollection<T> GetCollection<T>(string collectionName);
}