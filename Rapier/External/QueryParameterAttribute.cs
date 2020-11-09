using System;

namespace Rapier.External
{

    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    //public class QueryParameterAttribute : Attribute
    //{
    //    public QueryParameterAttribute(params string[] descriptor)
    //        => Descriptor = descriptor;

    //    public string[] Descriptor { get; set; }

    //}
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute(string entity, string node)
            => (Entity, Node) = (entity, node);

        public string Entity { get; set; }
        public string Node { get; set; }
    }
}
