using Rapier.Configuration.Settings;
using Rapier.External;
using Rapier.External.Enums;
using Rapier.External.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapier.Configuration
{
    public static class ConfigurationOptionsExtensions
    {
        private static readonly List<ActionEndpointSettings> _controllerMethods;

        static ConfigurationOptionsExtensions()
        {
            _controllerMethods = typeof(IRapierController<,>)
                .GetMethods()
                .Select(x => new ActionEndpointSettings(x.Name))
                .ToList();
        }

        public static ControllerEndpointSettings Add(this RapierConfigurationOptions options,
            Type entityType, string route)
        {
            var controller = new ControllerEndpointSettings
            {
                Route = route,
                ActionSettingsCollection = _controllerMethods
            };
            options.EndpointSettingsCollection.Add(entityType, controller);
            return controller;
        }

        public static ControllerEndpointSettings AuthorizeAction(this ControllerEndpointSettings controller,
            string actionMethodName, AuthorizationCategory category, string policy = null)
        {
            controller.ActionSettingsCollection.FirstOrDefault(x => x.ActionMethod
                .EndsWith(actionMethodName, StringComparison.OrdinalIgnoreCase))
                .Authorize(category, policy);
            return controller;
        }

        public static TEndpoint Authorize<TEndpoint>(
            this TEndpoint endpoint, AuthorizationCategory category,
            string policy = null) where TEndpoint : IEndpoint
        {
            endpoint.AuthorizeableEndpoint = new()
            {
                Category = category,
                Policy = policy
            };
            return endpoint;
        }

        //public static ActionEndpointSettings Action(this ControllerEndpointSettings controller, string actionMethodName)
        //    => controller.ActionSettingsCollection.FirstOrDefault(x => x.ActionMethod
        //        .EndsWith(actionMethodName, StringComparison.OrdinalIgnoreCase));

        public static ControllerEndpointSettings ExpandMembersExplicitly(
            this ControllerEndpointSettings controller,
            params string[] members)
        {
            controller.AutoExpandMembers = false;
            controller.ExplicitExpandedMembers = members;
            return controller;
        }
    }
}
