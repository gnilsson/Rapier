using Rapier.Configuration.Settings;
using Rapier.Internal.Utility;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class RequestProviderItems
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string,
            ExpressionUtility.ConstructorDelegate>> Parameters { get; set; }

        public PaginationSettings PaginationSettings { get; set; }
    }
}
