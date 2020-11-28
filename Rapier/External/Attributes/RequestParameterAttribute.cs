using Rapier.External.Enums;
using System;

namespace Rapier.External.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequestParameterAttribute : Attribute
    {
        public RequestParameterAttribute(
            string entityProperty = null, RequestParameterMode mode = 0)
            => (EntityProperty, Mode) = (entityProperty, mode);
        public string EntityProperty { get; }
        public RequestParameterMode Mode { get; }
    }
}
