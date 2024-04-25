using LangChain.Databases.Mongo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LangChain.Databases.Mongo.Client
{
    public interface IMongoDBClient
    {
        Task BatchDeactivate<T>(Expression<Func<T, bool>> filter) where T : BaseEntity;

        IEnumerable<TProjected> GetSync<T, TProjected>(Expression<Func<T, bool>> filter,
           Expression<Func<T, TProjected>> projectionExpression) where T : BaseEntity;

        Task InsertAsync<T>(T entity) where T : BaseEntity;
    }
}
