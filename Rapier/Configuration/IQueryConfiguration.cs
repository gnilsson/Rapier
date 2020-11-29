using System.Collections.Generic;

namespace Rapier.Configuration
{
    public interface IQueryConfiguration
    {
        public IEnumerable<string> ExpandMembers { get; }
    }
}
