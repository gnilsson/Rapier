using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Rapier.Configuration.Settings;
using Rapier.External;
using System.Collections.Generic;
using System.Reflection;

namespace Rapier.Configuration
{
    public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IEnumerable<IEntitySettings> _settings;
        public GenericTypeControllerFeatureProvider(IEnumerable<IEntitySettings> settings) 
            => _settings = settings;
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var setting in _settings)
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
