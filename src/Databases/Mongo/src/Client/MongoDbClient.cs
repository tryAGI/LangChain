﻿using LangChain.Databases.Mongo.Model;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LangChain.Databases.Mongo.Client;

public class MongoDbClient(IMongoContext mongoContext) : IMongoDbClient
{
    public async Task BatchDeactivate<T>(Expression<Func<T, bool>> filter) where T : BaseEntity
    {
        var entityIds = (await Get(filter, p => p.Id).ConfigureAwait(false)).ToList();
        var update = Builders<T>.Update.Set(e => e.IsActive, false);

        await GetCollection<T>().UpdateManyAsync(e => entityIds.Contains(e.Id), update).ConfigureAwait(false);
    }

    public async Task<IEnumerable<TProjected>> Get<T, TProjected>(Expression<Func<T, bool>> filter,
        Expression<Func<T, TProjected>> projectionExpression) where T : BaseEntity
    {
        return await GetCollection<T>()
            .Find(filter)
            .Project(projectionExpression)
            .ToListAsync().ConfigureAwait(false);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return mongoContext.GetCollection<T>(GetCollectionName(typeof(T)));
    }

    private static string GetCollectionName(Type documentType)
    {
        return (documentType.GetCustomAttributes(
                       typeof(BsonCollectionAttribute),
                       true)
                   .FirstOrDefault() as BsonCollectionAttribute)?.CollectionName ??
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
        entity = entity ?? throw new ArgumentNullException(nameof(entity));

        await GetCollection<T>().InsertOneAsync(entity).ConfigureAwait(false);
    }
}