using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Rapier.External.Enums;
using System.Linq;

namespace Rapier.Configuration
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        private readonly RapierConfigurationOptions _config;
        public GenericControllerRouteConvention(RapierConfigurationOptions config) => _config = config;
        // Something strange happens here on compile, gets init twice?
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType && _config.GeneratedControllers)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var setting = _config.EntitySettingsCollection.FirstOrDefault(x => x.ResponseType == genericType);

                if (!string.IsNullOrWhiteSpace(setting.ControllerRoute))
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel { Template = setting.ControllerRoute },
                    });
                    controller.ControllerName = $"{setting.EntityType.Name} Controller";

                    foreach (var action in controller.Actions)
                        if (setting.AuthorizeableEndpoints.TryGetValue(
                            $"{controller.ControllerType.FullName}.{action.ActionMethod.Name}", out var endpoint))
                            if (endpoint.Category != AuthorizationCategory.None)
                                action.Filters.Add(
                                    new AuthorizeFilter(
                                        endpoint.Category == AuthorizationCategory.Custom ?
                                        endpoint.Policy : string.Empty));
                }
            }
        }
    }
}
