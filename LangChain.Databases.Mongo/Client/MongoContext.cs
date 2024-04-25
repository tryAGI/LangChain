using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Databases.Mongo.Client
{
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoContext(IDatabaseConfiguration databaseConfiguration)
        {
            var client = new MongoClient(databaseConfiguration.ConnectionString);
            _mongoDatabase = client.GetDatabase(databaseConfiguration.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _mongoDatabase.GetCollection<T>(collectionName);
        }
    }
}
