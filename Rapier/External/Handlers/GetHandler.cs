using AutoMapper;
using MediatR;
using Rapier.External.Models;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using Rapier.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.Handlers
{
    public class GetHandler<TEntity, TRequest, TResponse> :
        IRequestHandler<TRequest, PagedResponse<TResponse>>
        where TEntity : IEntity
        where TRequest : QueryReciever, IRequest<PagedResponse<TResponse>>
    {
        internal readonly IRepository<TEntity, TResponse> _repository;
        internal readonly IMapper _mapper;
        internal readonly IUriService _uriService;

        public GetHandler(
            IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IUriService uriService) =>
            (_repository, _mapper, _uriService) =
            (repositoryWrapper.Get<TEntity, TResponse>(), mapper, uriService);

        public virtual async Task<PagedResponse<TResponse>> Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            var queryData = await _repository.GetQueriedResultAsync(request, cancellationToken);

            return PaginationUtility.CreatePaginatedResponse(
                _uriService, request, queryData.Items, queryData.Total);
        }
    }
}
