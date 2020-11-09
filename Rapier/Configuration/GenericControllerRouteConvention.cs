using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Rapier.External;
using System.Linq;
using System.Reflection;

namespace Rapier.Configuration
{
    public class GenericControllerRouteConvention : IControllerModelConvention
    {
        private readonly RapierConfigurationOptions _config;
        public GenericControllerRouteConvention(RapierConfigurationOptions config)
        {
            _config = config;
        }
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType && _config.GeneratedControllers)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var route = _config.EntitySettings
                    .FirstOrDefault(x => x.ResponseType == genericType).ControllerRoute;

                if(!string.IsNullOrWhiteSpace(route))
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel { Template = route }
                    });
                }
            }
        }
    }
}
