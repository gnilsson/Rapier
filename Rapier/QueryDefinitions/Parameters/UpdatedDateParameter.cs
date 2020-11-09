using Rapier.External;

namespace Rapier.QueryDefinitions.Parameters
{
    public class UpdatedDateParameter : DateParameter
    {
        public override void Set(string value)
        {
            base.Set(value);
            TableReferenceChildren = new[] { nameof(Entity.UpdatedDate) };
        }
    }
}
