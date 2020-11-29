namespace Rapier.QueryDefinitions.Parameters
{
    public class ExpandParameter
    {
        public string[] Nodes { get; }
        public ExpandParameter(string expandable)
        {
            Nodes = expandable.Split('.');
        }
    }
}
