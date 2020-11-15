using Rapier.External;

namespace Rapier.QueryDefinitions.Parameters
{
    public class UpdatedDateParameter : DateParameter
    {
        public UpdatedDateParameter(string value)
        {
            base.Set(value);
            TableReferenceChildren = new[] { nameof(IEntity.UpdatedDate) };
        }
    }
}
