using Rapier.External.Models;
using Rapier.QueryDefinitions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public interface IRepository<TEntity,TResponse>
    {
        ValueTask<TEntity> FindAsync(
            Guid entityId,
            CancellationToken token);
        Task<List<TEntity>> GetManyByConditionAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken token);
        //Task<TDto> GetFirstOrDefaultAsync(
        //                Expression<Func<TEntity, bool>> predicate);
        //Task<QueryResult<TDto>> GetQueriedResultAsync(
        //                        QueryReciever<GetRequest> queryReciever);
        Task<TEntity> GetSingleByConditionAsync(
            Expression<Func<TEntity, bool>> predicate,
            string includeNavigation,
            CancellationToken token);
        void Delete(TEntity entity, CancellationToken token);
        Task CreateAsync(TEntity entity, CancellationToken token);
        Task<IQueryResult<TResponse>> GetQueriedResultAsync(
            QueryReciever queryReciever, CancellationToken token);
    }
}
