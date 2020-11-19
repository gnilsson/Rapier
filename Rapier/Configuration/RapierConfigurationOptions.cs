using Rapier.Configuration.Settings;
using Rapier.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rapier.Configuration
{
    public class RapierConfigurationOptions
    {
        public Type AssemblyType { get; set; }
        public Type ContextType { get; set; }
        public IEnumerable<IEntitySettings> EntitySettingsCollection { get; set; }
        public Dictionary<Type, ControllerEndpointSettings> EndpointSettingsCollection { get; }
        public bool GeneratedControllers { get; set; } = true;
        public bool InterfaceDiscovery { get; set; } = true;
        public bool RoutesByAttribute { get; set; } = false;
        public Type ExtendedRepository { get; set; }
        public PaginationSettings PaginationSettings { get; set; }

        private readonly IList<ActionEndpointSettings> _controllerMethods;

        public RapierConfigurationOptions()
        {
            _controllerMethods = typeof(IRapierController<,>)
                .GetMethods()
                .Select(x => new ActionEndpointSettings(x.Name))
                .ToList();
            EndpointSettingsCollection = new();
        }

        public ControllerEndpointSettings Add(Type entityType, string route)
        {
            var controller = new ControllerEndpointSettings 
            { Route = route, ActionSettingsCollection = _controllerMethods };
            EndpointSettingsCollection.Add(entityType, controller);
            return controller;
        }
    }
}
