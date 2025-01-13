using GenericMongoRepository.DAL.Models;

namespace GenericMongoRepository.DAL.Repositories.BattlePassesRepo;

public interface IBattlePassRepo<TEntity, TId> : IMongoCrudRepository<TEntity, TId> where TEntity : MongoEntity<TId>
{
	public Task<string> SayHello(string name);
}