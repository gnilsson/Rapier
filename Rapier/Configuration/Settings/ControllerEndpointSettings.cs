using Rapier.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration.Settings
{
    public class ControllerEndpointSettings : EndpointBase, IEndpoint
    {
        public string Route { get; init; }
        public IList<ActionEndpointSettings> ActionSettingsCollection { get; init; }

        public ControllerEndpointSettings()
        {
        }

        public ActionEndpointSettings Action(string actionMethodName)
            => ActionSettingsCollection.FirstOrDefault(x => x.ActionMethod
                .EndsWith(actionMethodName, StringComparison.OrdinalIgnoreCase));
    }
}
