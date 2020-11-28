using Microsoft.OpenApi.Models;
using Rapier.Descriptive;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rapier.Configuration
{
    public class RapierOperationFilter : IOperationFilter
    {
        private readonly SemanticsDefiner _semanticsDefiner;

        public RapierOperationFilter(SemanticsDefiner semanticsDefiner) => _semanticsDefiner = semanticsDefiner;
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerKey = context.ApiDescription.ActionDescriptor.RouteValues[Keys.RouteValue.Controller];
            var actionKey = context.ApiDescription.ActionDescriptor.RouteValues[Keys.RouteValue.Action];

            if (_semanticsDefiner.ActionNames.TryGetValue($"{controllerKey}.{actionKey}", out var newAction))
                context.ApiDescription.ActionDescriptor.RouteValues[Keys.RouteValue.Action] = newAction;
        }
    }
}
