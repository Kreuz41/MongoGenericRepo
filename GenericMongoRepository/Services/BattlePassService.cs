using GenericMongoRepository.DAL.Models.Entities;
using GenericMongoRepository.DAL.Repositories;
using GenericMongoRepository.DAL.Repositories.BattlePassesRepo;

namespace GenericMongoRepository.Services;

public class BattlePassService
{
	private readonly IBattlePassRepo<BattlePass, long> _repository;

	public BattlePassService(IBattlePassRepo<BattlePass, long> repository)
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

	public Task<string> SayHelloAsync(string name)
	{
		return _repository.SayHello(name);
	}
}