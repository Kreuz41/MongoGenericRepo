using GenericMongoRepository.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace GenericMongoRepository.DAL.Models.Entities;

[MongoEntity("BattlePasses", "id")]
public class BattlePass : MongoEntity<long>
{
	[BsonElement("id")]
	public override required long ObjectId { get; set; }
	
	[BsonElement("description")] 
	public string Description { get; set; } = string.Empty;
	
	[BsonElement("name")]
	public string Name { get; set; } = string.Empty;

	[BsonElement("startBundle")] 
	public List<string> StartBundle { get; set; } = [];

	[BsonElement("tier")] 
	public string Tier { get; set; } = string.Empty;

	[BsonElement("unlockedLevels")] 
	public int UnlockedLevels { get; set; }
}