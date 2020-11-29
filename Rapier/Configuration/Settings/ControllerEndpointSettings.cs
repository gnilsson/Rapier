using System.Collections.Generic;

namespace Rapier.Configuration.Settings
{
    public class ControllerEndpointSettings : EndpointBase, IEndpoint
    {
        public string Route { get; init; }
        public IList<ActionEndpointSettings> ActionSettingsCollection { get; init; }
        public bool AutoExpandMembers { get; set; } = true;
        public string[] ExplicitExpandedMembers { get; set; }
    }
}
