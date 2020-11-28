using System;

namespace Rapier.External.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdCollectionAttribute : Attribute
    {
        public IdCollectionAttribute(Type entityType) => EntityType = entityType;
        public Type EntityType { get; }
    }
}
