using Rapier.Configuration.Settings;
using System;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class RapierConfigurationOptions
    {
        public Type AssemblyType { get; set; }
        public Type ContextType { get; set; }
        public IEnumerable<IEntitySettings> EntitySettingsCollection { get; set; }
        public IDictionary<Type, ControllerEndpointSettings> EndpointSettingsCollection { get; set; }
        public bool GeneratedControllers { get; set; } = true;
        public bool InterfaceDiscovery { get; set; } = true;
        public bool RoutesByAttribute { get; set; } = false;
        public Type ExtendedRepository { get; set; }
        public PaginationSettings PaginationSettings { get; set; }
    }
}
