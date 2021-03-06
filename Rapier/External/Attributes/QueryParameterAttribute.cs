using System;

namespace Rapier.External.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute(Type parameterType, string[] navigationNodes = null) 
            => (ParameterType, NavigationNodes) = (parameterType, navigationNodes);
        public Type ParameterType { get; }
        public string[] NavigationNodes { get; }
    }
}
