using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Rapier.External;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public class Repository<TEntity, TResponse, TContext> :
                 RepositoryConcept<TContext>,
                 IRepository<TEntity, TResponse>
                 where TEntity : Entity
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
            QueryReciever queryReciever)
        {
            var query = SetQuery().AsNoTracking();
            var count = await query.CountAsync();
            if (_querier != null)
                query = query.Where(_querier(queryReciever.Parameters));
            if (_orderer != null)
                query = _orderer(query, queryReciever.OrderByParameter);

            return new QueryResult<TResponse>(
                count,
                await query
                .ApplyPaging(queryReciever.PaginationQuery)
                .ProjectTo<TResponse>(_mapper.ConfigurationProvider)
                .ToListAsync());
        }
        //public async Task<QueryResult<Response>> GetQueriedResultAsync(
        //             QueryReciever queryReciever)
        //{
        //    //var query = SetQuery().AsNoTracking();
        //    //var count = await query.CountAsync();
        //    //var dtos = await
        //    //    _orderer(query
        //    //        .ApplyPaging(queryReciever.PaginationQuery)
        //    //        .Where(_querier(queryReciever.Parameters)),
        //    //        queryReciever.OrderByParameter)
        //    //    .ProjectTo<TResponse>(_mapper.ConfigurationProvider)
        //    //    .ToListAsync();
        //    //return new QueryResult<TResponse>(count, dtos);
        //    //var query = SetQuery().AsNoTracking();
        //    //var count = await query.CountAsync();
        //    //if (_includer != null)
        //    //    query = query.IgnoreAutoIncludes(); //_includer(query.IgnoreAutoIncludes());

        //    var query = SetQuery().AsNoTracking();
        //    var count = await query.CountAsync();
        //    if (_querier != null)
        //        query = query.Where(_querier(queryReciever.Parameters));
        //    if (_orderer != null)
        //        query = _orderer(query, queryReciever.OrderByParameter);

        //    return new QueryResult<TResponse>(
        //        count,
        //        await query
        //        .ApplyPaging(queryReciever.PaginationQuery)
        //        .Proj ProjectTo<TResponse>(_mapper.ConfigurationProvider)
        //        .ToListAsync());
        //}
        public async ValueTask<TEntity> FindAsync(Guid entityId)
            => await Set().FindAsync(entityId);
        public async Task<List<TEntity>> GetManyByConditionAsync(
            Expression<Func<TEntity, bool>> predicate)
           => await FindByCondition(predicate).ToListAsync();
        private IQueryable<TEntity> FindByCondition(
             Expression<Func<TEntity, bool>> predicate)
           => SetQuery().Where(predicate);

        public Task<TEntity> GetSingleByConditionAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(TEntity entity)
           => await Set().AddAsync(entity);


    }
}
