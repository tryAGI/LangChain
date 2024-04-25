namespace LangChain.Databases.Mongo.Client;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BsonCollectionAttribute(string collectionName) : Attribute
{
    public string CollectionName { get; } = collectionName;
}