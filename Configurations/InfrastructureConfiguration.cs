using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDBRepository.Data;
using MongoDBRepository.Exceptions;
using MongoDBRepository.Interfaces;
using MongoDBRepository.Providers;

namespace MongoDBRepository.Configurations;

public static class InfrastructureConfiguration
{
    public static void ConfigureDatabase<TKey>(this IServiceCollection services, IConfiguration configuration) where TKey : notnull
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        ConventionRegistry.Register(
            "Ignore",
            new ConventionPack
            {
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true),
            },
            t => true
        );
        
        using ILoggerFactory factory = LoggerFactory.Create(b => b.AddConsole());
        ILogger logger = factory.CreateLogger("Program");
        services.AddSingleton(logger);

        bool isTenantEnabled = bool.Parse(configuration.GetSection("MongoDB")["IsTenantEnabled"] ?? throw new MissingMongoDBConfigurationException("IsTenantEnabled"));

        services.AddScoped<UserInfoProvider>();
        services.AddScoped<IMongoContext, MongoContext<TKey>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
       
        if(!isTenantEnabled)
        {
            BsonClassMap.RegisterClassMap<BaseEntity<TKey>>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.TenantId);
            });
        }
    }
    
    public static void ConfigureDatabase<TSub, TId, TTenant>(this IServiceCollection services, IConfiguration configuration) 
        where TId : notnull
        where TTenant : notnull
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        ConventionRegistry.Register(
            "Ignore",
            new ConventionPack
            {
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true),
            },
            t => true
        );
        
        using ILoggerFactory factory = LoggerFactory.Create(b => b.AddConsole());
        ILogger logger = factory.CreateLogger("Program");
        services.AddSingleton(logger);

        bool isTenantEnabled = bool.Parse(configuration.GetSection("MongoDB")["IsTenantEnabled"] ?? throw new MissingMongoDBConfigurationException("IsTenantEnabled"));

        services.AddScoped<UserInfoProvider>();
        services.AddScoped<IMongoContext, MongoContext<TSub, TTenant>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
       
        if(!isTenantEnabled)
        {
            BsonClassMap.RegisterClassMap<BaseEntity<TId, TTenant>>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.TenantId);
            });
        }
    }
}