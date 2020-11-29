using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class QueryConfiguration<T> : IQueryConfiguration
    {
        public IEnumerable<string> ExpandMembers { get; }

        public QueryConfiguration(ICollection<string> expandMembers)
        {
            ExpandMembers = expandMembers;
        }
    }
}
