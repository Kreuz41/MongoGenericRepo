using GenericMongoRepository.DAL.Models;

namespace GenericMongoRepository.DAL.Repositories;

public interface IMongoCrudRepository<TEntity, in TEntityId> 
	where TEntity : MongoEntity<TEntityId>
{
	Task<TEntity?> GetById(TEntityId id, CancellationToken cancellationToken = default);
	Task<IEnumerable<TEntity>> GetList(int skip, int take, CancellationToken cancellationToken = default);
	Task<long> Count(CancellationToken cancellationToken = default);
	Task Insert(TEntity entity, CancellationToken cancellationToken = default);
	Task Update(TEntityId id, TEntity updatedEntity, CancellationToken cancellationToken = default);
	Task Upsert(TEntityId id, TEntity entity, CancellationToken cancellationToken = default);
	Task Delete(TEntityId id, CancellationToken cancellationToken = default);
}