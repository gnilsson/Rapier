using Rapier.Descriptive;
using System;

namespace Rapier.QueryDefinitions.Parameters
{
    public abstract class DateParameter : IParameter
    {
        public object Value { get; private set; }
        public string[] ParentNavigationProperties { get; internal set; }
        public string[] NavigationProperties { get; internal set; }
        public string Method { get; private set; }
        public virtual void Set(string value)
        {
            Value = DateTime.Parse(value);
            Method = QueryMethods.CallDateTimeCompare;
        }
    }
}
