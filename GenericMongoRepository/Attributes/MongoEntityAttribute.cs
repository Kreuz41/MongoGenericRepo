namespace GenericMongoRepository.Attributes;

public class MongoEntityAttribute : Attribute
{
	public string CollectionName { get; set; }
	public string IdName { get; set; }
	
	public MongoEntityAttribute(string collectionName, string idName)
	{
		CollectionName = collectionName;
		IdName = idName;
	}
}