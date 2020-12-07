using Rapier.External.Models;
using Rapier.QueryDefinitions;
using Rapier.Services;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Internal.Utility
{
    public static class PaginationUtility
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(
                      IUriService uriService,
                      QueryReciever request,
                      ICollection<T> response,
                      int? total)
        {
            var pagination = request.PaginationQuery;
            var nextPage = pagination.PageNumber >= 1
                ? uriService.GetUri(request.RequestRoute,
                 new PaginationQuery
                 {
                     PageNumber = pagination.PageNumber + 1,
                     PageSize = pagination.PageSize
                 }).ToString()
                : null;

            var previousPage = pagination.PageNumber - 1 >= 1
                ? uriService.GetUri(request.RequestRoute,
                 new PaginationQuery
                 {
                     PageNumber = pagination.PageNumber - 1,
                     PageSize = pagination.PageSize
                 }).ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber
                : (int?)null,
                PageSize = pagination.PageSize > response.Count ? response.Count
                : pagination.PageSize >= 1 ? pagination.PageSize
                : (int?)null,
                NextPage = total > pagination.PageSize ? nextPage
                : null,
                PreviousPage = previousPage,
                Total = total,
                Errors = request.Errors
            };
        }
    }
}
