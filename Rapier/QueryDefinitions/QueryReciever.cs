using Rapier.External.Models;
using Rapier.QueryDefinitions.Parameters;
using System.Collections.Generic;

namespace Rapier.QueryDefinitions
{
    public abstract class QueryReciever
    {
        public QueryReciever(GetRequest query) => Query = query;

        public GetRequest Query { get; }
        public ICollection<IParameter> Parameters { get; set; }
        public PaginationQuery PaginationQuery { get; set; }
        public string RequestRoute { get; set; }
        public OrderByParameter OrderByParameter { get; set; }
        public string[] ExpandMembers { get; set; }
        public ICollection<string> Errors { get; set; }
    }
}
