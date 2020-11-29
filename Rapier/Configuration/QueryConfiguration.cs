using System;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class QueryConfiguration<T> : IQueryConfiguration
    {
        public ICollection<string[]> IncluderDetails { get; }
        public IEnumerable<string> ExpandMembers { get; }

        public QueryConfiguration(ICollection<string> expandMembers)
        {
            ExpandMembers = expandMembers;
            // ExpandMembers = expandMembers ?? Array.Empty<string>();
        }


    }
}
