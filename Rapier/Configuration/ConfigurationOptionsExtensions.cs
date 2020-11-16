using Rapier.Configuration.Settings;
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
        //public static ControllerEndpointSettings Add(Type entityType, string route)
        //{
        //    var controller = new ControllerEndpointSettings
        //    { Route = route, ActionSettingsCollection = _controllerMethods };
        //    EndpointSettingsCollection.Add(entityType, controller);
        //    return controller;
        //}

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

        //public static ActionEndpointSettings Action(
        //    this ControllerEndpointSettings controller, string actionMethodName)
        //    => controller.ActionSettingsCollection
        //    .FirstOrDefault(x => x.ActionMethod.EndsWith(actionMethodName, StringComparison.OrdinalIgnoreCase));

    }
}
