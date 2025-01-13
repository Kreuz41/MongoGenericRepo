using System.Reflection;
using GenericMongoRepository.Attributes;
using GenericMongoRepository.DAL.Repositories;
using MongoDB.Driver;

namespace GenericMongoRepository.Extensions;

public static class MongoRepositoryRegistration
{
    public static IServiceCollection AddMongoRepositories(this IServiceCollection services, IMongoDatabase database)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        var entityTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && 
                        t.GetCustomAttribute<MongoEntityAttribute>() != null)
            .ToList();
        
        var customRepositoryTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, BaseType.IsGenericType: true } &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(MongoCrudRepository<,>))
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var mongoCollectionNameAttribute = entityType.GetCustomAttribute<MongoEntityAttribute>();
            if (mongoCollectionNameAttribute == null) continue;
            
            var idType = entityType.BaseType?.GenericTypeArguments.FirstOrDefault();
            if (idType == null) throw new InvalidOperationException($"Unable to determine ID type for {entityType.Name}");
            
            var customRepositoryType = customRepositoryTypes?
                .FirstOrDefault(repo => repo.BaseType?.GenericTypeArguments[0] == entityType);
            
            if (customRepositoryType != null)
            {
                var interfaceType = typeof(IMongoCrudRepository<,>).MakeGenericType(entityType, idType);
                AddMongoRepository(services, database, customRepositoryType, 
                    interfaceType, mongoCollectionNameAttribute.CollectionName, mongoCollectionNameAttribute.IdName);

                continue;
            }
            
            var defaultRepositoryType = typeof(MongoCrudRepository<,>).MakeGenericType(entityType, idType);
            var defaultInterfaceType = typeof(IMongoCrudRepository<,>).MakeGenericType(entityType, idType);

            AddMongoRepository(services, database, defaultInterfaceType, defaultRepositoryType, 
                mongoCollectionNameAttribute.CollectionName, mongoCollectionNameAttribute.IdName);
        }

        return services;
    }

    private static void AddMongoRepository(
        IServiceCollection services,
        IMongoDatabase database,
        Type interfaceType,
        Type implementationType,
        string collectionName,
        string idFieldName)
    {
        services.AddSingleton(interfaceType, sp =>
        {
            var loggerType = typeof(ILogger<>).MakeGenericType(implementationType);
            var logger = sp.GetRequiredService(loggerType);
            return Activator.CreateInstance(implementationType, logger, database, collectionName, idFieldName)
                   ?? throw new InvalidOperationException($"Unable to create instance of {implementationType.FullName}");
        });
    }
}