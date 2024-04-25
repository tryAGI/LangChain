using MongoDB.Driver;

namespace LangChain.Databases.Mongo.Client;

public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _mongoDatabase;

    public MongoContext(IDatabaseConfiguration databaseConfiguration)
    {
        databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        
        var client = new MongoClient(databaseConfiguration.ConnectionString);
        _mongoDatabase = client.GetDatabase(databaseConfiguration.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        name = name ?? throw new ArgumentNullException(nameof(name));
        
        return _mongoDatabase.GetCollection<T>(name);
    }
}