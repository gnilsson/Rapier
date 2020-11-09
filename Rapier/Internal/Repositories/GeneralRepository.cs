using Microsoft.EntityFrameworkCore;
using Rapier.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public class GeneralRepository<TContext> :
                 RepositoryConcept<TContext>,
                 IGeneralRepository
                 where TContext : DbContext
    {

        public GeneralRepository(
            TContext dbContext) 
            : base(dbContext) 
        { }

        private DbSet<TEntity> Set<TEntity>() where TEntity : class
            => DbContext.Set<TEntity>();
        private DbSet<Entity> Set(Type entityType)
            => (DbSet<Entity>)typeof(DbContext)
            .GetMethod("Set")
            .MakeGenericMethod(entityType)
            .Invoke(DbContext, null);


        private IQueryable<TEntity> SetQuery<TEntity>() where TEntity : class
            => DbContext.Set<TEntity>();

        //public async Task<TDto> GetFirstByConditionAsync<TDto, TEntity>(
        //                      Expression<Func<TEntity, bool>> predicate)
        //                      where TEntity : class
        //    => await SetQuery<TEntity>()
        //            .AsNoTracking()
        //            .Where(predicate)
        //            .ProjectTo<TDto>(_mapper.ConfigurationProvider)
        //            .FirstOrDefaultAsync();

        public async Task<TEntity> GetFirstByConditionAsync<TEntity>(
                           Expression<Func<TEntity, bool>> predicate)
                           where TEntity : class
            => await SetQuery<TEntity>()
                    .AsNoTracking()
                    .Where(predicate)
                    .FirstOrDefaultAsync();

        public IQueryable<TEntity> FindByCondition<TEntity>(
            Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
            => SetQuery<TEntity>().Where(predicate);

        public async ValueTask<TEntity> FindAsync<TEntity>(
            Guid entityId)
            where TEntity : Entity
          => await Set<TEntity>().FindAsync(entityId);
        public async ValueTask<Entity> FindAsync(
            Type entityType,
            Guid entityId)
        => await Set(entityType).FindAsync(entityId);

        public async Task<List<TEntity>> GetManyAsync<TEntity>(
            IEnumerable<Guid> entityIds)
            where TEntity : Entity
             => await FindByCondition<TEntity>(e => entityIds.Contains(e.Id))
                .ToListAsync();

        public async Task CreateAsync<TEntity>(
            TEntity entity)
            where TEntity : class
           => await Set<TEntity>().AddAsync(entity);

        public async Task CreateManyAsync<TEntity>(
            IEnumerable<TEntity> entities)
            where TEntity : class
           => await Set<TEntity>().AddRangeAsync(entities);

        public void Delete<TEntity>(
            TEntity entity)
            where TEntity : class
           => Set<TEntity>().Remove(entity);


    }
}
