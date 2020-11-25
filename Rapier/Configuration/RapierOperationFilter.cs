using Microsoft.OpenApi.Models;
using Rapier.Descriptive;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rapier.Configuration
{
    public class RapierOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //var actionKey = "action";
            //var controllerKey = "controller";

            //var controller = context.ApiDescription.ActionDescriptor.RouteValues[controllerKey];
            //var entity = controller.Split('C')[0];
            //var action = context.ApiDescription.ActionDescriptor.RouteValues[actionKey];

            //if (context.MethodInfo.Name == DefaultActions.Get)
            //{
            //    context.ApiDescription.ActionDescriptor.RouteValues[actionKey] = $"{action}{entity}s";
            //}
            //else if (context.MethodInfo.Name == DefaultActions.GetById)
            //{
            //    var getById = action.Split('B');
            //    context.ApiDescription.ActionDescriptor.RouteValues[actionKey] = $"{getById[0]}{entity}{getById[1]}";
            //}
            //else
            //{
            //    context.ApiDescription.ActionDescriptor.RouteValues[actionKey] = $"{action}{entity}";
            //}
        }
    }
}
