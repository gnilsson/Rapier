using MediatR;
using Microsoft.AspNetCore.Http;
using Rapier.Configuration;
using Rapier.Configuration.Settings;
using Rapier.Descriptive;
using Rapier.External.Models.Records;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using Rapier.QueryDefinitions.Parameters;
using System;
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
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, QueryParameterShell>> _parameters;
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
                .Select(x => x.Value.Constructor(
                    _httpContext.Request.Query[x.Key].ToString(), x.Value.NavigationArgs) as IParameter)
                .ToList();

            request.PaginationQuery = new PaginationQuery(request.Query, _paginationSettings);
            request.RequestRoute = _httpContext.Request.Path;

            if (!string.IsNullOrWhiteSpace(request.Query.OrderBy))
                AddOrderByParameter(request);

            if (!string.IsNullOrWhiteSpace(request.Query.Expand))
                AddExpandMembers(request);

            return await next();
        }

        private void AddOrderByParameter(QueryReciever request)
        {
            if (!request.Query.OrderBy.Contains(":"))
            {
                AddError(request, ErrorMessage.Query.OrderParameterSeperator);
                return;
            }

            var orderQuery = request.Query.OrderBy.Split(":");
            if (!orderQuery[0].Contains(OrderParameterDescriptor.Ascending, StringComparison.OrdinalIgnoreCase) &&
                !orderQuery[0].Contains(OrderParameterDescriptor.Descending, StringComparison.OrdinalIgnoreCase))
            {
                AddError(request, ErrorMessage.Query.OrderParameterDescriptor);
                return;
            }

            if (!_querySemantics.DefaultFields.Contains(orderQuery[0]))
            {
                AddError(request, ErrorMessage.Query.OrderParameterField);
                return;
            }

            request.OrderByParameter = new OrderByParameter(orderQuery);
        }

        private void AddExpandMembers(QueryReciever request)
        {
            var expandMembers = new List<string>();
            foreach (var member in request.Query.Expand.Split('.'))
            {
                if (_querySemantics.RelationalFields.Contains(member))
                {
                    expandMembers.Add(member);
                    continue;
                }

                AddError(request, ErrorMessage.Query.ExpandParameterField);
            }

            request.ExpandMembers = expandMembers.ToArray();
        }

        private static void AddError(QueryReciever request, string errorMessage)
        {
            request.Errors ??= new List<string>();
            request.Errors.Add(errorMessage);
        }
    }
}
