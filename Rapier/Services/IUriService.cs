using Rapier.External.Models;
using System;

namespace Rapier.Services
{
    public interface IUriService
    {
        public Uri GetByIdUri(string requestRoute, string id);
        public Uri GetUri(string requestRoute, IPaginateable paginationData = null);
    }
}
