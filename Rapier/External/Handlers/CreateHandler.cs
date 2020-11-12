using AutoMapper;
using MediatR;
using Rapier.CommandDefinitions;
using Rapier.External.Models;
using Rapier.Internal.Repositories;
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

        private readonly IModifier<TEntity, TRequest>.CreateDelegate _creator;

        public CreateHandler(IRepositoryWrapper repositoryWrapper,
                             IMapper mapper,
                             IModifier<TEntity, TRequest> modifier) =>
            (_repositoryWrapper, _repository, _mapper, _creator) =
            (repositoryWrapper, repositoryWrapper.Get<TEntity, TResponse>(), mapper, modifier.Creator);

        public virtual async Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(_creator(request), cancellationToken);
            await _repositoryWrapper.SaveAsync();
            return _mapper.Map<TResponse>(
                await _repository.FindAsync(request.Id, cancellationToken));
        }
    }
}
