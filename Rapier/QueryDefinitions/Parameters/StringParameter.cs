using Rapier.Descriptive;

namespace Rapier.QueryDefinitions.Parameters
{
    public abstract class StringParameter : IParameter
    {
        public object Value { get; protected set; }
        public string[] TableReferenceParents { get; protected set; }
        public string[] TableReferenceChildren { get; protected set; }
        public string Method { get; protected set; }
        public virtual void Set(string value)
        {
            Value = value;
            Method = QueryMethods.CallStringContains;
        }
    }
}
