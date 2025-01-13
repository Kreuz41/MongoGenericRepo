using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GenericMongoRepository.DAL.Models;

public abstract class MongoEntity<TId>
{
	public ObjectId Id { get; set; }
	public abstract required TId ObjectId { get; set; }
}