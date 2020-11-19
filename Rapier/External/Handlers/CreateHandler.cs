using AutoMapper;
using MediatR;
using Rapier.CommandDefinitions;
using Rapier.External.Models;
using Rapier.Internal.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.Handlers
{
    public class CreateHandler<TEntity, TRequest, TResponse> : 
        IRequestHandler<TRequest, TResponse>
        where TEntity : IEntity
        where TRequest : ICommand, IRequest<TResponse>
    {
        internal readonly IRepository<TEntity, TResponse> _repository;
        internal readonly IRepositoryWrapper _repositoryWrapper;
        internal readonly IMapper _mapper;

        private readonly Func<TRequest, TEntity> _create;
        public CreateHandler(IRepositoryWrapper repositoryWrapper,
                             IMapper mapper,
                             IModifier<TEntity, TRequest> modifier) =>
            (_repositoryWrapper, _repository, _mapper, _create) =
            (repositoryWrapper, repositoryWrapper.Get<TEntity, TResponse>(), mapper, modifier.Create);

        public virtual async Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken)
        {
            var entity = _create(request);
            await _repository.CreateAsync(entity, cancellationToken);
            await _repositoryWrapper.SaveAsync();
            return _mapper.Map<TResponse>(entity);
        }
    }
}
