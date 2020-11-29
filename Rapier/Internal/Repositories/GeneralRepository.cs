using Microsoft.EntityFrameworkCore;
using Rapier.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public class GeneralRepository<TContext> :
                 RepositoryConcept<TContext>,
                 IGeneralRepository
                 where TContext : DbContext
    {

        public GeneralRepository(TContext dbContext) : base(dbContext)
        { }

        private DbSet<TEntity> Set<TEntity>() where TEntity : class
            => DbContext.Set<TEntity>();

        private IQueryable<TEntity> SetQuery<TEntity>() where TEntity : class
            => DbContext.Set<TEntity>();

        public async Task<TEntity> GetFirstByConditionAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken token)
            where TEntity : class
            => await SetQuery<TEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(predicate, token);

        public IQueryable<TEntity> FindByCondition<TEntity>(
            Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
            => SetQuery<TEntity>().Where(predicate);

        public async ValueTask<TEntity> FindAsync<TEntity>(
            Guid entityId,
            CancellationToken token)
            where TEntity : class, IEntity
          => await Set<TEntity>().FindAsync(new object[] { entityId }, token);

        public async Task<List<TEntity>> GetManyAsync<TEntity>(
            IEnumerable<Guid> entityIds,
            CancellationToken token)
            where TEntity : class, IEntity
             => await FindByCondition<TEntity>(e => entityIds.Contains(e.Id))
                .ToListAsync(token);

        public async Task<List<TEntity>> GetManyAsync<TEntity>(
            IEnumerable<Guid> entityIds,
            string includeNavigation,
            CancellationToken token)
            where TEntity : class, IEntity
             => await FindByCondition<TEntity>(e => entityIds.Contains(e.Id))
                .Include(includeNavigation)
                .ToListAsync(token);

        public async Task CreateAsync<TEntity>(
            TEntity entity, CancellationToken token)
            where TEntity : class
           => await Set<TEntity>().AddAsync(entity, token);

        public async Task CreateManyAsync<TEntity>(
            IEnumerable<TEntity> entities, CancellationToken token)
            where TEntity : class
           => await Set<TEntity>().AddRangeAsync(entities, token);

        public void Delete<TEntity>(
            TEntity entity)
            where TEntity : class
           => Set<TEntity>().Remove(entity);
    }
}
