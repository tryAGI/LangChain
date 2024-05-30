using MongoDB.Driver;

namespace LangChain.Databases.Mongo.Client;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>(string name);
    Task<List<string>> GetCollections();
    IMongoDatabase GetDatabase();
}