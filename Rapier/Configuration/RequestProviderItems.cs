using Rapier.Configuration.Settings;
using Rapier.External.Models.Records;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class RequestProviderItems
    {
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, QueryParameterShell>> Parameters { get; init; }

        public PaginationSettings PaginationSettings { get; init; }
    }
}
