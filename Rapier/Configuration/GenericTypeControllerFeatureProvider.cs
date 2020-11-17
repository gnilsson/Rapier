using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Rapier.External;
using System.Collections.Generic;
using System.Reflection;

namespace Rapier.Configuration
{

    public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly RapierConfigurationOptions _config;
        public GenericTypeControllerFeatureProvider(RapierConfigurationOptions config) => _config = config;
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var setting in _config.EntitySettingsCollection)
            {
                feature.Controllers.Add(
                    typeof(RapierController<,,>)
                    .MakeGenericType(
                        setting.ResponseType,
                        setting.QueryRequestType,
                        setting.CommandRequestType)
                    .GetTypeInfo());
            }
        }
    }
}
