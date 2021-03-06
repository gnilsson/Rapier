using Rapier.Internal.Utility;

namespace Rapier.External.Models.Records
{
    public record QueryParameterShell(ExpressionUtility.ConstructorDelegate Constructor, string[] NavigationArgs);
}
