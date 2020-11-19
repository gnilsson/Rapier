using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rapier.External;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Internal.Repositories
{
    public class RepositoryWrapper<TContext> :
                 IRepositoryWrapper
                 where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ConcurrentDictionary<string, object> _repoCache;
        private readonly IReadOnlyDictionary<string, RepositoryConstructContainer> _repoConstructorContainer;
        private IGeneralRepository _general;

        public RepositoryWrapper(
            TContext context,
            IMapper mapper,
            IReadOnlyDictionary<string,
            RepositoryConstructContainer> repoCtor) =>
            (_dbContext, _mapper, _repoCache, _repoConstructorContainer) =
            (context, mapper, new ConcurrentDictionary<string, object>(), repoCtor);

        public IGeneralRepository General
            => _general ??= new GeneralRepository<TContext>(_dbContext);

        public IRepository<TEntity, TResponse> Get<TEntity, TResponse>() where TEntity : IEntity
            => (IRepository<TEntity, TResponse>)(
                _repoCache.ContainsKey(typeof(TEntity).Name) ?
                _repoCache[typeof(TEntity).Name] :
                _repoCache.GetOrAdd(
                typeof(TEntity).Name,
                Fetch<TEntity, TResponse>()));

        private IRepository<TEntity, TResponse> Fetch<TEntity, TResponse>()
        {
            var construct = _repoConstructorContainer
                .FirstOrDefault(x => x.Key == typeof(TEntity).Name).Value;
            return construct.RepositoryConstructor(
                _dbContext, _mapper, construct.QueryConfiguration) 
                as IRepository<TEntity, TResponse>;
        }
        public async Task SaveAsync() => await _dbContext.SaveChangesAsync();
    }
}
