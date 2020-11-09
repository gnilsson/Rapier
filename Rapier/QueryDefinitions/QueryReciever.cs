using MediatR;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.QueryDefinitions
{
    public abstract class QueryReciever
    {    
        public QueryReciever(GetRequest query)
        {
            Query = query;
        }

        public GetRequest Query { get; }
        public ICollection<IParameter> Parameters { get; set; }
        public PaginationQuery PaginationQuery { get; set; }
        public string RequestRoute { get; set; }
        public OrderByParameter OrderByParameter { get; set; }

        internal void Add<T>(
            string value)
            where T : IParameter, new()
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            var parameter = new T();
            parameter.Set(value);
            Parameters.Add(parameter);
        }
    }
}
