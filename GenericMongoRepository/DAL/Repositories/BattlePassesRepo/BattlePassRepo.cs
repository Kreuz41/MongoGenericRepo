using GenericMongoRepository.DAL.Models.Entities;
using MongoDB.Driver;

namespace GenericMongoRepository.DAL.Repositories.BattlePassesRepo;

public class BattlePassRepo : MongoCrudRepository<BattlePass, long>, IBattlePassRepo<BattlePass, long>
{
	public BattlePassRepo(ILogger<MongoCrudRepository<BattlePass, long>> logger, IMongoDatabase database, string collectionName, string idFieldName) : 
		base(logger, database, collectionName, idFieldName)
	{
	}

	public Task<string> SayHello(string name)
	{
		return Task.FromResult("Hello " + name);
	}
}