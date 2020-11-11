using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rapier.External;
using Rapier.External.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                    controller.ApiExplorer = new ApiExplorerModel { GroupName = $"{setting.EntityType.Name} Controller" };

                    foreach (var action in controller.Actions)
                    {
                        var key = $"{controller.ControllerType.FullName}.{action.ActionMethod.Name}";
                        if (setting.AuthorizeableEndpoints.ContainsKey(key))
                        {
                            if (setting.AuthorizeableEndpoints[key] == AuthorizationCategory.Default)
                                action.Filters.Add(new AuthorizeFilter());
                        }
                    }
                }
            }
        }
    }
}
