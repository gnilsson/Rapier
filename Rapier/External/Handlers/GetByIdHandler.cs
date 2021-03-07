using AutoMapper;
using MediatR;
using Rapier.External.Models;
using Rapier.Internal.Repositories;
using System.Threading;
using System.Threading.Tasks;
namespace Rapier.External.Handlers
{
    public class GetByIdHandler<TEntity, TRequest, TResponse> :
        IRequestHandler<TRequest, TResponse>
        where TEntity : IEntity
        where TRequest : GetByIdQuery<TResponse>
    {
        internal readonly IRepository<TEntity, TResponse> _repository;
        internal readonly IMapper _mapper;
        public GetByIdHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper) =>
            (_repository, _mapper) =
            (repositoryWrapper.Get<TEntity, TResponse>(), mapper);

        public virtual async Task<TResponse> Handle(
            TRequest request, CancellationToken cancellationToken)
        {
            var entityResponse = await _repository.GetFirstOrDefaultAsync(
                x => x.Id == request.Id, cancellationToken);

            return entityResponse ?? default;
        }
    }
}
