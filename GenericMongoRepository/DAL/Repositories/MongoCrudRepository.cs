using GenericMongoRepository.DAL.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GenericMongoRepository.DAL.Repositories;

public class MongoCrudRepository<TEntity, TEntityId> : IMongoCrudRepository<TEntity, TEntityId> 
    where TEntity : MongoEntity<TEntityId>
{
    private readonly IMongoCollection<TEntity> _collection;
    private readonly ILogger<MongoCrudRepository<TEntity, TEntityId>> _logger;
    private readonly string _idFieldName;
    private readonly string _collectionName;

    public MongoCrudRepository(
        ILogger<MongoCrudRepository<TEntity, TEntityId>> logger,
        IMongoDatabase database, 
        string collectionName,
        string idFieldName)
    {
        _logger = logger;
        _idFieldName = idFieldName;
        _collectionName = collectionName;
        _collection = database.GetCollection<TEntity>(collectionName);
    }

    public async Task<TEntity?> GetById(TEntityId id, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Getting from {_collectionName} by id: {id}");

        try
        {
            var filter = Builders<TEntity>.Filter.Eq(_idFieldName, id);

            var entity = await _collection
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            _logger.LogInformation(entity is null
                ? $"{_collectionName} with id: {id} not found"
                : $"{_collectionName} with id: {id} found");

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving from {_collectionName}");
            throw;
        }
    }

    public async Task<IEnumerable<TEntity>> GetList(int skip, int take, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Getting list from {_collectionName} with pagination: skip={skip}, take={take}");

        try
        {
            var entities = await _collection
                .Find(_ => true)
                .Skip(skip)
                .Limit(take)
                .ToListAsync(cancellationToken);

            _logger.LogInformation($"{entities.Count} from {_collectionName} retrieved");

            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while retrieving paginated from {_collectionName}");
            throw;
        }
    }

    public async Task<long> Count(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Counting all from {_collectionName}");

        try
        {
            var count = await _collection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
            _logger.LogInformation($"Total count of {count} from {_collectionName}");

            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while counting from {_collectionName}");
            throw;
        }
    }

    public async Task Insert(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Inserting new in {_collectionName}");

        try
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            _logger.LogInformation($"In {_collectionName} successfully inserted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while inserting in {_collectionName}");
            throw;
        }
    }

    public async Task Update(TEntityId id, TEntity updatedEntity, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Updating in {_collectionName} with id: {id}");

        try
        {
            var filter = Builders<TEntity>.Filter.Eq(_idFieldName, id);
            var result = await _collection.ReplaceOneAsync(
                filter,
                updatedEntity,
                cancellationToken: cancellationToken);

            _logger.LogInformation(result.ModifiedCount > 0 
                ? $"In {_collectionName} with id: {id} successfully updated" 
                : $"In {_collectionName} with id: {id} not found for update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating in {_collectionName}");
            throw;
        }
    }

    public async Task Upsert(TEntityId id, TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Upserting in {_collectionName} with id: {id}");

        try
        {
            var filter = Builders<TEntity>.Filter.Eq(_idFieldName, id);
            
            var result = await _collection.ReplaceOneAsync(
                filter,
                entity,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

            _logger.LogInformation(result.UpsertedId != null
                ? $"In {_collectionName} with id: {id} successfully inserted (upsert)"
                : $"In {_collectionName} with id: {id} successfully updated (upsert)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while upserting in {_collectionName}");
            throw;
        }
    }

    public async Task Delete(TEntityId id, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace($"Deleting from {_collectionName} with id: {id}");

        try
        {
            var filter = Builders<TEntity>.Filter.Eq(_idFieldName, id);
            var result = await _collection.DeleteOneAsync(
                filter,
                cancellationToken);
            
            _logger.LogInformation(result.DeletedCount > 0 
                ? $"From {_collectionName} with id: {id} successfully deleted" 
                : $"From {_collectionName} with id: {id} not found for deletion");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting from {_collectionName}");
            throw;
        }
    }
}
