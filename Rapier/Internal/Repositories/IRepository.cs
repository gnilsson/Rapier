using Rapier.External.Models;
using Rapier.QueryDefinitions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public interface IRepository<TEntity,TResponse>
    {
        ValueTask<TEntity> FindAsync(Guid entityId);
        Task<List<TEntity>> GetManyByConditionAsync(
            Expression<Func<TEntity, bool>> predicate);
        //Task<TDto> GetFirstOrDefaultAsync(
        //                Expression<Func<TEntity, bool>> predicate);
        //Task<QueryResult<TDto>> GetQueriedResultAsync(
        //                        QueryReciever<GetRequest> queryReciever);
        Task<TEntity> GetSingleByConditionAsync(
            Expression<Func<TEntity, bool>> predicate);
        void Delete(TEntity entity);
        Task CreateAsync(TEntity entity);
        Task<IQueryResult<TResponse>> GetQueriedResultAsync(
            QueryReciever queryReciever);
    }
}
