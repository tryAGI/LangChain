using LangChain.Databases.Mongo.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Databases.Mongo.Client
{
    public class MongoDBClient: IMongoDBClient
    {
        private readonly IMongoContext _mongoContext;

        public MongoDBClient(IMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }
        public async Task BatchDeactivate<T>(Expression<Func<T, bool>> filter) where T : BaseEntity
        {
            var entityIds = (await Get(filter, p => p.Id)).ToList();
            var update = Builders<T>.Update.Set(e => e.IsActive, false);

            await GetCollection<T>().UpdateManyAsync(e => entityIds.Contains(e.Id), update);
        }

        public async Task<IEnumerable<TProjected>> Get<T, TProjected>(Expression<Func<T, bool>> filter,
            Expression<Func<T, TProjected>> projectionExpression) where T : BaseEntity
        {
            return await GetCollection<T>()
                .Find(filter)
                .Project(projectionExpression)
                .ToListAsync();
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _mongoContext.GetCollection<T>(GetCollectionName(typeof(T)));
        }

        private static string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                           typeof(BsonCollectionAttribute),
                           true)
                       .FirstOrDefault())?.CollectionName ??
                   throw new InvalidOperationException(
                       $"Collection name must to be specified using {nameof(BsonCollectionAttribute)}");
        }

        public IEnumerable<TProjected> GetSync<T, TProjected>(Expression<Func<T, bool>> filter,
          Expression<Func<T, TProjected>> projectionExpression) where T : BaseEntity
        {
            return GetCollection<T>()
                .Find(filter)
                .Project(projectionExpression)
                .ToList();
        }

        public async Task InsertAsync<T>(T entity) where T : BaseEntity
        {
            Guard.AgainstNullArgument(entity, nameof(entity));
            await GetCollection<T>().InsertOneAsync(entity);
        }
    }
}
