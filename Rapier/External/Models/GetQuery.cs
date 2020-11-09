using MediatR;
using Rapier.QueryDefinitions;

namespace Rapier.External.Models
{
    public class GetQuery<TQuery, TResponse> :
        QueryReciever,
        IRequest<PagedResponse<TResponse>>
        where TQuery : GetRequest
    {
        public GetQuery(
            TQuery request) :
            base(request)
        { }
    }
}
