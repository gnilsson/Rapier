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
    public class UpdateHandler<TEntity, TRequest, TResponse> :
        IRequestHandler<TRequest, TResponse>
        where TEntity : IEntity
        where TRequest : ICommand, IRequest<TResponse>
    {
        internal readonly IRepository<TEntity, TResponse> _repository;
        internal readonly IRepositoryWrapper _repositoryWrapper;
        internal readonly IMapper _mapper;

        private readonly Action<TEntity, TRequest> _update;

        public UpdateHandler(IRepositoryWrapper repositoryWrapper,
                             IMapper mapper,
                             IModifier<TEntity, TRequest> modifier) =>
            (_repositoryWrapper, _repository, _mapper, _update) =
            (repositoryWrapper, repositoryWrapper.Get<TEntity, TResponse>(), mapper, modifier.Update);

        public virtual async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            var entity = request.IncludeNavigation == null ?
               await _repository.FindAsync(request.Id, cancellationToken) :
               await _repository.GetSingleByConditionAsync(
                   x => x.Id == request.Id, request.IncludeNavigation, cancellationToken);

            if (entity == null) return default;

            _update(entity, request);
            await _repositoryWrapper.SaveAsync();
            return _mapper.Map<TResponse>(entity);
        }
    }
}


