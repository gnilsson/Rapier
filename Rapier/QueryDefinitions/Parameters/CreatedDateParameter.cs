using Rapier.External;

namespace Rapier.QueryDefinitions.Parameters
{
    public class CreatedDateParameter : DateParameter
    {
        public CreatedDateParameter(string value)
        {
            base.Set(value);
            NavigationProperties = new[] { nameof(IEntity.CreatedDate) };
        }
    }
}
