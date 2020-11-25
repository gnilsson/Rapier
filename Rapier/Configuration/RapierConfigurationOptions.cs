using Rapier.Configuration.Settings;
using Rapier.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration
{
    public class RapierConfigurationOptions
    {
        private readonly IList<ActionEndpointSettings> _controllerMethods;
        public Type AssemblyType { get; set; }
        public Type ContextType { get; set; }
        public IEnumerable<IEntitySettings> EntitySettingsCollection { get; set; }
        public Dictionary<Type, ControllerEndpointSettings> EndpointSettingsCollection { get; }
        public bool GeneratedControllers { get; set; } = true;
        public bool InterfaceDiscovery { get; set; } = true;
        public bool RoutesByAttribute { get; set; } = false;
        public Type ExtendedRepositoryType { get; set; }
        public PaginationSettings PaginationSettings { get; set; }
        public bool AutoExpandEntities { get; set; } = true;

        public RapierConfigurationOptions()
        {
            _controllerMethods = typeof(IRapierController<,>)
                .GetMethods()
                .Select(x => new ActionEndpointSettings(x.Name))
                .ToList();
            EndpointSettingsCollection = new();
        }
    }
}
