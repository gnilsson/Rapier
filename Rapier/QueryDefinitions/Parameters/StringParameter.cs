using Rapier.Descriptive;

namespace Rapier.QueryDefinitions.Parameters
{
    public class StringParameter : IParameter
    {
        public string[] ParentNavigationProperties { get; protected set; }
        public string[] NavigationProperties { get; protected set; }
        public object Value { get; private set; }
        public string Method { get; private set; }

        public virtual void Set(string value)
        {
            Value = value;
            Method = QueryMethod.CallStringContains;
        }

        public StringParameter(string value, string[] navigationNodes)
        {
            this.Set(value);
            NavigationProperties = navigationNodes;
        }
    }
}
