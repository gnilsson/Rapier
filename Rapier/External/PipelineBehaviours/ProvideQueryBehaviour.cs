using MediatR;
using Microsoft.AspNetCore.Http;
using Rapier.Configuration;
using Rapier.Configuration.Settings;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using Rapier.QueryDefinitions.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.PipelineBehaviours
{
    internal sealed class ProvideQueryBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : QueryReciever
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string,
            ExpressionUtility.ConstructorDelegate>> _parameters;
        private readonly PaginationSettings _paginationSettings;
        private readonly HttpContext _httpContext;
        private readonly SemanticsDefiner.Query<TResponse> _querySemantics;

        public ProvideQueryBehaviour(RequestProviderItems providerItems,
            IHttpContextAccessor accessor, SemanticsDefiner semanticsDefiner)
        {
            _parameters = providerItems.Parameters;
            _paginationSettings = providerItems.PaginationSettings;
            _httpContext = accessor.HttpContext;
            _querySemantics = semanticsDefiner.GetQuery<TResponse>();
        }
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            request.Parameters = _parameters
                .FirstOrDefault(x => x.Key == request.Query.GetType().Name).Value
                .Where(x => _httpContext.Request.Query.ContainsKey(x.Key))
                .Select(x => x.Value(_httpContext.Request.Query[x.Key]) as IParameter)
                .ToList();

            request.PaginationQuery = new PaginationQuery(request.Query, _paginationSettings);
            request.RequestRoute = _httpContext.Request.Path;

            request.OrderByParameter =
                string.IsNullOrWhiteSpace(request.Query.OrderBy) ||
                !request.Query.OrderBy.Contains(":") ?
                null : new OrderByParameter(request.Query.OrderBy);

            if (!string.IsNullOrWhiteSpace(request.Query.Expand))
            {
                var members = request.Query.Expand.Split('.');
                if (members.All(x => _querySemantics.Members.Contains(x)))
                    request.ExpandMembers = members;
            }

            return await next();
        }
    }
}
