using Rapier.External;
using Rapier.External.Enums;
using Rapier.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration.Settings
{
    public class ControllerEndpointSettings
    {
        public string Route { get; set; }
        public IList<ActionEndpointSettings> ActionSettingsCollection { get; }
        public AuthorizeableEndpoint AuthorizeableEndpoint { get; set; }
        public ControllerEndpointSettings()
        {
            ActionSettingsCollection = typeof(IRapierController<,,>)
                .GetMethods()
                .Select(x => new ActionEndpointSettings(x.Name))
                .ToList();
            AuthorizeableEndpoint = new();
        }
    }
}
