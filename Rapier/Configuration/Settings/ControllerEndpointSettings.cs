using System.Collections.Generic;

namespace Rapier.Configuration.Settings
{
    public class ControllerEndpointSettings : EndpointBase, IEndpoint
    {
        public string Route { get; init; }
        public IList<ActionEndpointSettings> ActionSettingsCollection { get; init; }
    }
}
