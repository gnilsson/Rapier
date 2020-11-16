using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Rapier.External;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public class Repository<TEntity, TResponse, TContext> :
                 RepositoryConcept<TContext>,
                 IRepository<TEntity, TResponse>
                 where TEntity : class, IEntity
                 where TContext : DbContext
    {

        private readonly IMapper _mapper;
        private readonly Func<IQueryable<TEntity>, OrderByParameter, IOrderedQueryable<TEntity>> _orderer;
        private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _includer;
        private readonly QueryInstructions<TEntity>.QueryDelegate _querier;
        public Repository(
            TContext context,
            IMapper mapper,
            QueryManager<TEntity> queryManager) :
            base(context) =>
            (_mapper, _orderer, _includer, _querier) =
            (mapper, queryManager.Orderer, queryManager.Includer, queryManager.Querier);

        private DbSet<TEntity> Set()
            => DbContext.Set<TEntity>();
        private IQueryable<TEntity> SetQuery()
            => DbContext.Set<TEntity>();
        public async Task<IQueryResult<TResponse>> GetQueriedResultAsync(
            QueryReciever queryReciever, CancellationToken token)
        {
            var query = SetQuery().AsNoTracking();
            var count = await query.CountAsync(token);
            if (_querier != null)
                query = query.Where(_querier(queryReciever.Parameters));
            if (_orderer != null)
                query = _orderer(query, queryReciever.OrderByParameter);

            return new QueryResult<TResponse>(
                count,
                await query
                .ApplyPaging(queryReciever.PaginationQuery)
                .ProjectTo<TResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(token));
        }

        public async Task<TResponse> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken token)
          => await SetQuery()
                    .AsNoTracking()
                    .Where(predicate)
                    .ProjectTo<TResponse>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(token);

        public async ValueTask<TEntity> FindAsync(
            Guid entityId, CancellationToken token)
            => await Set().FindAsync(new object[] { entityId }, token);
        public async Task<List<TEntity>> GetManyByConditionAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken token)
           => await FindByCondition(predicate).ToListAsync(token);
        private IQueryable<TEntity> FindByCondition(
             Expression<Func<TEntity, bool>> predicate)
           => SetQuery().Where(predicate);

        public async Task<TEntity> GetSingleByConditionAsync(
            Expression<Func<TEntity, bool>> predicate,
            string includeNavigation,
            CancellationToken token)
          => await SetQuery()
                .Where(predicate)
                .Include(includeNavigation)
                .FirstOrDefaultAsync(token);

        public void Delete(TEntity entity, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(
            TEntity entity, CancellationToken token)
           => await Set().AddAsync(entity, token);


    }
}
