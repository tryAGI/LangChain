using LangChain.Databases.Mongo.Model;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace LangChain.Databases.Mongo.Client;

public interface IMongoDbClient
{
    Task BatchDeactivate<T>(Expression<Func<T, bool>> filter)
        where T : BaseEntity;
    bool CollectionExists(string collectionName);
    Task<bool> CollectionExistsAsync(string collectionName);
    Task<IMongoCollection<T>> CreateCollection<T>(string collectionName);
    Task DropCollectionAsync(string collectionName);
    Task<IEnumerable<TProjected>> Get<T, TProjected>(Expression<Func<T, bool>> filter, Expression<Func<T, TProjected>> projectionExpression) where T : BaseEntity;
    IMongoCollection<T> GetCollection<T>();
    Task<List<string>> GetCollections();
    IEnumerable<TProjected> GetSync<T, TProjected>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TProjected>> projectionExpression)
        where T : BaseEntity;

    Task InsertAsync<T>(T entity)
        where T : BaseEntity;
}