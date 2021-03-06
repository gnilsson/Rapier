﻿using Rapier.QueryDefinitions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public interface IRepository<TEntity, TResponse>
    {
        ValueTask<TEntity> FindAsync(
            Guid entityId,
            CancellationToken token);
        Task<List<TEntity>> GetManyByConditionAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken token);
        Task<TResponse> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken token);
        Task<TEntity> GetSingleByConditionAsync(
            Expression<Func<TEntity, bool>> predicate,
            string includeNavigation,
            CancellationToken token);
        void Delete(TEntity entity);
        Task CreateAsync(TEntity entity, CancellationToken token);
        Task<IQueryResult<TResponse>> GetQueriedResultAsync(
            QueryReciever queryReciever, CancellationToken token);
    }
}
