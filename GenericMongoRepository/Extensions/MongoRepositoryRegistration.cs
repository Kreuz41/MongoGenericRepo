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

        var entityTypes = GetEntityTypesWithAttribute<MongoEntityAttribute>(assembly);
        var customRepositoryTypes = GetCustomRepositoryTypes(assembly);

        foreach (var entityType in entityTypes)
        {
            var attribute = entityType.GetCustomAttribute<MongoEntityAttribute>();
            if (attribute == null) continue;

            var idType = entityType.BaseType?.GenericTypeArguments.FirstOrDefault();
            if (idType == null)
                throw new InvalidOperationException($"Unable to determine ID type for {entityType.Name}");

            var customRepositoryType = FindCustomRepositoryType(customRepositoryTypes, entityType);

            if (customRepositoryType != null)
            {
                var interfaceType = GetCustomRepositoryInterface(customRepositoryType, entityType, idType);
                AddMongoRepository(services, database, interfaceType, customRepositoryType, 
                    attribute.CollectionName, attribute.IdName);
            }
            else
            {
                RegisterDefaultRepository(services, database, entityType, idType, attribute.CollectionName, attribute.IdName);
            }
        }

        return services;
    }

    private static List<Type> GetEntityTypesWithAttribute<TAttribute>(Assembly assembly) where TAttribute : Attribute
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<TAttribute>() != null)
            .ToList();
    }

    private static List<Type> GetCustomRepositoryTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(MongoCrudRepository<,>))
            .ToList();
    }

    private static Type? FindCustomRepositoryType(List<Type> customRepositoryTypes, Type entityType)
    {
        return customRepositoryTypes
            .FirstOrDefault(repo => repo.BaseType?.GenericTypeArguments[0] == entityType);
    }

    private static Type GetCustomRepositoryInterface(Type customRepositoryType, Type entityType, Type idType)
    {
        var baseInterface = typeof(IMongoCrudRepository<,>).MakeGenericType(entityType, idType);
        return customRepositoryType.GetInterfaces().First(t => t != baseInterface);
    }

    private static void RegisterDefaultRepository(
        IServiceCollection services,
        IMongoDatabase database,
        Type entityType,
        Type idType,
        string collectionName,
        string idFieldName)
    {
        var repositoryType = typeof(MongoCrudRepository<,>).MakeGenericType(entityType, idType);
        var interfaceType = typeof(IMongoCrudRepository<,>).MakeGenericType(entityType, idType);

        AddMongoRepository(services, database, interfaceType, repositoryType, collectionName, idFieldName);
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