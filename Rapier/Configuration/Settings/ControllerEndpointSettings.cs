using Rapier.External;
using Rapier.External.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration.Settings
{
    public class ControllerEndpointSettings
    {
        public string Route { get; set; }
        public AuthorizationCategory Authorize { get; set; } = 0;
        public IList<ActionEndpointSettings> ActionSettingsCollection { get; }

        public ControllerEndpointSettings()
        {
            ActionSettingsCollection = typeof(IRapierController<,,>)
                .GetMethods()
                .Select(x => new ActionEndpointSettings(x.Name))
                .ToList();
        }
    }
}
