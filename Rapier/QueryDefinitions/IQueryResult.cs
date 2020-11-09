using System.Collections.Generic;

namespace Rapier.QueryDefinitions
{
    public interface IQueryResult<TResponse>
    {
        public int? Total { get; }
        public ICollection<TResponse> Items { get; }
    }
}
