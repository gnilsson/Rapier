namespace Rapier.QueryDefinitions.Parameters
{
    public interface IParameter
    {
        public string[] TableReferenceParents { get; }
        public string[] TableReferenceChildren { get; }
        public object Value { get; }
        public string Method { get; }
        public void Set(string value);
    }
}
