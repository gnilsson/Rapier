namespace Rapier.QueryDefinitions.Parameters
{
    public interface IParameter
    {
        public string[] ParentNavigationProperties { get; }
        public string[] NavigationProperties { get; }
        public object Value { get; }
        public string Method { get; }
        public void Set(string value);
    }
}
