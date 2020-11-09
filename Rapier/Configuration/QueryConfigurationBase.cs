using System.Collections.Generic;

namespace Rapier.Configuration
{
    public abstract class QueryConfigurationBase<TEntity>
    {
        public List<string[]> IncluderDetails { get; protected set; }

    }
}
