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
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public class ExtendedRepository<TEntity, TResponse, TContext> :
        Repository<TEntity, TResponse, TContext>
        where TEntity : Entity
        where TContext : DbContext
    {
        private readonly IMapper _mapper;
        private readonly Func<IQueryable<TEntity>, OrderByParameter, IOrderedQueryable<TEntity>> _orderer;
        private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _includer;
        private readonly QueryInstructions<TEntity>.QueryDelegate _querier;
        public ExtendedRepository(
            TContext context,
            IMapper mapper,
            QueryManager<TEntity> queryManager) :
            base(context, mapper, queryManager) =>
            (_mapper, _orderer, _includer, _querier) =
            (mapper, queryManager.Orderer, queryManager.Includer, queryManager.Querier);
    }
}
