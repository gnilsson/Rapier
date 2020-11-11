using System;

namespace Rapier.External
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute(string entity, string node)
            => (Entity, Node) = (entity, node);

        public string Entity { get; set; }
        public string Node { get; set; }
    }
}
