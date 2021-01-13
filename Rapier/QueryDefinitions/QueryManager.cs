using Rapier.Configuration;
using Rapier.External;
using System.Linq;

namespace Rapier.QueryDefinitions
{
    public class QueryManager<TEntity> where TEntity : class, IEntity
    {
        public QueryInstructions<TEntity>.QueryDelegate Querier { get; }
        public string[] ExpandMembers { get; }
        public QueryManager(IQueryConfiguration config)
        {
            var instructions = new QueryInstructions<TEntity>();
            Querier = instructions.Query;
            ExpandMembers = config.ExpandMembers?.ToArray();
        }
    }
}
