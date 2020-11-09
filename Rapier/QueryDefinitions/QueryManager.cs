﻿using Microsoft.EntityFrameworkCore.Query;
using Rapier.Configuration;
using Rapier.External;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Linq;

namespace Rapier.QueryDefinitions
{
    public class QueryManager<TEntity> where TEntity : Entity
    {
        //   public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Includer { get; private set; }
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> Includer { get; private set; }

        public QueryInstructions<TEntity>.QueryDelegate Querier { get; private set; }

        public Func<IQueryable<TEntity>, OrderByParameter, IOrderedQueryable<TEntity>> Orderer { get; private set; }
        public QueryManager(
            IQueryConfiguration config,
            QueryConfigurationGeneral generalConfig)
        {
            var instructions = new QueryInstructions<TEntity>(config, generalConfig);
            Querier = instructions.Querier;
            Orderer = instructions.Orderer;
            Includer = instructions.Includer();
        }
    }
}
