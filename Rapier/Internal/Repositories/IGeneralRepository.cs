using Rapier.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public interface IGeneralRepository
    {
        //Task<TDto> GetFirstByConditionAsync<TDto, TEntity>(
        //                    Expression<Func<TEntity, bool>> predicate)
        //                    where TEntity : class;
        Task<TEntity> GetFirstByConditionAsync<TEntity>(
                            Expression<Func<TEntity, bool>> predicate)
                            where TEntity : class;
        IQueryable<TEntity> FindByCondition<TEntity>(
                            Expression<Func<TEntity, bool>> predicate)
                            where TEntity : class;
        ValueTask<TEntity> FindAsync<TEntity>(Guid entityId)
                            where TEntity : Entity;
        ValueTask<Entity> FindAsync(Type entityType, Guid entityId);
        Task<List<TEntity>> GetManyAsync<TEntity>(
                            IEnumerable<Guid> entityIds)
                            where TEntity : Entity;
        Task CreateAsync<TEntity>(TEntity entity)
                            where TEntity : class;
        Task CreateManyAsync<TEntity>(IEnumerable<TEntity> entities)
                            where TEntity : class;
        void Delete<TEntity>(TEntity entity)
                            where TEntity : class;
    }
}
