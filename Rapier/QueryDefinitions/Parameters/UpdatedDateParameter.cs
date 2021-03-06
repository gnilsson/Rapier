using Rapier.External;

namespace Rapier.QueryDefinitions.Parameters
{
    public class UpdatedDateParameter : DateParameter
    {
        public UpdatedDateParameter(string value, string[] navigationNodes = null)
            : base(value, navigationNodes ?? new[] { nameof(IEntity.UpdatedDate) })
        { }
    }
}
