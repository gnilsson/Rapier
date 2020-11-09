using Rapier.External.Models;
using Rapier.Internal.Utility;
using System;

namespace Rapier.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        private const string PageNumber = "pageNumber";
        private const string PageSize = "pageSize";
        public UriService(string baseUri) => _baseUri = baseUri;

        public Uri GetByIdUri(string requestRoute, string id)
        {
            return new Uri(_baseUri + requestRoute
                .Replace(requestRoute
                    .Substring(requestRoute
                        .IndexOf("{")), id));
        }

        public Uri GetUri(string requestRoute, IPaginateable paginationData = null)
        {
            var uri = new Uri(_baseUri + requestRoute);
            if (paginationData == null)
                return uri;

            return new Uri(uri.ToString()
                .AppendQueryString(PageNumber, paginationData.PageNumber.ToString())
                .AppendQueryString(PageSize, paginationData.PageSize.ToString()));
        }
    }
}
