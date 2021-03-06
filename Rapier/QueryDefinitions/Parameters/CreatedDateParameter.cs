using Rapier.External;

namespace Rapier.QueryDefinitions.Parameters
{
    public class CreatedDateParameter : DateParameter
    {
        public CreatedDateParameter(string value, string[] navigationNodes = null)
            : base(value, navigationNodes ?? new[] { nameof(IEntity.CreatedDate) })
        { }
    }
}
