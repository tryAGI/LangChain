﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.Core.Configuration;

namespace LangChain.Databases.Mongo.Client;

public class MongoContext : IMongoContext
{
    protected readonly IMongoDatabase _mongoDatabase;

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

    public IMongoDatabase GetDatabase()
    {
        return _mongoDatabase;
    }

    public async Task<List<string>> GetCollections()
    {
        List<string> collectionNames = new List<string>();
        var collections = await _mongoDatabase.ListCollectionsAsync();

        foreach (BsonDocument collection in await collections.ToListAsync<BsonDocument>())
        {
            string name = collection["name"].AsString;
            collectionNames.Add(name);
        }

        return collectionNames;
    }

}