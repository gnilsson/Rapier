using Rapier.Configuration;
using Rapier.External;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Linq;

namespace Rapier.QueryDefinitions
{
    public class QueryManager<TEntity> where TEntity : class, IEntity
    {
        public QueryInstructions<TEntity>.QueryDelegate Querier { get; }
        public Func<IQueryable<TEntity>, OrderByParameter, IOrderedQueryable<TEntity>> Orderer { get; }
        public string[] ExpandMembers { get; }
        public QueryManager(IQueryConfiguration config)
        {
            var instructions = new QueryInstructions<TEntity>(config);
            Querier = instructions.Query;
            Orderer = instructions.Order;
            ExpandMembers = config.ExpandMembers?.ToArray();
        }
    }
}
