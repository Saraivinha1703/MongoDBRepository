using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDBRepository.Exceptions;
using MongoDBRepository.Interfaces;
using MongoDBRepository.Providers;

namespace MongoDBRepository.Data;

public class MongoContext<TKey> : IMongoContext
{
    public MongoClient MongoClient { get; set; }
    public string DefaultDatabaseName { get; set; }
    public string? RoleToDefaultDatabase { get; set; } = null;
    public bool IsTenantEnabled { get; set; }
    public IMongoDatabase Database { get; set; }
    private readonly List<Func<Task>> _commands = [];
    public IClientSessionHandle? Session { get; set; }

    public MongoContext(IConfiguration configuration, UserInfoProvider userInfoProvider)
    {
        MongoClient client = new(configuration.GetSection("MongoDB")["ConnectionString"] ?? throw new MissingMongoDBConfigurationException("ConnectionString"));
        MongoClient = client;

        IsTenantEnabled = bool.Parse(configuration.GetSection("MongoDB")["IsTenantEnabled"] ?? throw new MissingMongoDBConfigurationException("ConnectionString"));
        DefaultDatabaseName = configuration.GetSection("MongoDB")["DefaultDatabaseName"] ?? throw new MissingMongoDBConfigurationException("DefaultDatabaseName");
        RoleToDefaultDatabase = configuration.GetSection("MongoDB")["RoleToDefaultDatabase"];

        IUserInfo<TKey>? userInfo = userInfoProvider.GetUserInformation<TKey>();

        if (userInfo is not null && userInfo.TenantId is not null && IsTenantEnabled)
        {
            bool toDefaultDatabase = userInfo.Roles.Contains(RoleToDefaultDatabase);
            string databaseName = toDefaultDatabase
                ? DefaultDatabaseName
                : userInfo.TenantId.ToString() ?? throw new UnableToGetTenantDatabaseNameException();
            
            Database = client.GetDatabase(databaseName);
        }
        else
        {
            Database = client.GetDatabase(DefaultDatabaseName);
        }        
    }
    public IMongoCollection<T> GetCollection<T>(string collectionName)
        => Database.GetCollection<T>(collectionName);

    public void AddCommand(Func<Task> func)
        => _commands.Add(func);

    public async Task<int> SaveChangesAsync()
    {
        using (Session = await MongoClient.StartSessionAsync())
        {
            Session.StartTransaction();

            IEnumerable<Task> commands = _commands.Select(c => c());

            await Task.WhenAll(commands);

            await Session.CommitTransactionAsync();
        }

        int count = _commands.Count;

        _commands.Clear();

        return count;
    }

    public void Dispose()
    {
        Session?.Dispose();
        GC.SuppressFinalize(this);
    }
}