using System.Collections.Generic;

namespace Rapier.Configuration
{
    public interface IQueryConfiguration
    {
        public ICollection<string[]> IncluderDetails { get; }
        public IEnumerable<string> ExpandMembers { get; }
    }
}
