using Rapier.QueryDefinitions.Parameters;
using System;

namespace Rapier.External.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute(string entity, string node)
            => (Entity, Node) = (entity, node);

        public string Entity { get; set; }
        public string Node { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryParameterAttribteNew : Attribute
    {
        public QueryParameterAttribteNew(IParameter parameter)
        {

        }
    }
}
