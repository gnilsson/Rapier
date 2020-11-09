using Rapier.External;

namespace Rapier.QueryDefinitions.Parameters
{
    public class CreatedDateParameter : DateParameter
    {
        public override void Set(string value)
        {
            base.Set(value);
            TableReferenceChildren = new[] { nameof(Entity.CreatedDate) };
        }
    }
}
