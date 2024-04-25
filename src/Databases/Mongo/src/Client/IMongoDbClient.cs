using LangChain.Databases.Mongo.Model;
using System.Linq.Expressions;

namespace LangChain.Databases.Mongo.Client;

public interface IMongoDbClient
{
    Task BatchDeactivate<T>(Expression<Func<T, bool>> filter)
        where T : BaseEntity;

    IEnumerable<TProjected> GetSync<T, TProjected>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TProjected>> projectionExpression)
        where T : BaseEntity;

    Task InsertAsync<T>(T entity)
        where T : BaseEntity;
}