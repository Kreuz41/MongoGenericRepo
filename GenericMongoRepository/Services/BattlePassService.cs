using GenericMongoRepository.DAL.Models.Entities;
using GenericMongoRepository.DAL.Repositories;

namespace GenericMongoRepository.Services;

public class BattlePassService
{
	private readonly IMongoCrudRepository<BattlePass, long> _repository;

	public BattlePassService(IMongoCrudRepository<BattlePass, long> repository)
	{
		_repository = repository;
	}

	public Task<BattlePass?> GetByIdAsync(long id)
	{
		return _repository.GetById(id);
	}

	public Task<IEnumerable<BattlePass>> GetAllAsync(int skip, int take)
	{
		return _repository.GetList(skip, take);
	}
}