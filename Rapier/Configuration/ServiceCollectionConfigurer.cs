using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rapier.Configuration.Settings;
using Rapier.External;
using Rapier.External.Enums;
using Rapier.External.Models;
using Rapier.Internal;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions.Parameters;
using Rapier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rapier.Configuration
{
    public static class ServiceCollectionConfigurer
    {
        public static RapierConfigurationOptions DiscoverInterfacesByEntityName(
            this RapierConfigurationOptions config)
        {
            var exportedTypes = config.AssemblyType.Assembly.GetExportedTypes();
            var entityTypes = exportedTypes
                .Where(x => x.BaseType == typeof(Entity) && !x.IsAbstract)
                .ToList()
                .OrderByDescending(x => x.Name.Length);

            var types = exportedTypes
                .Where(x => !x.IsAbstract &&
                x.BaseType == typeof(EntityResponse) ||
                x.BaseType == typeof(GetRequest) ||
                x.GetInterface(nameof(IModifyRequest)) != null ||
                x.GetInterface(nameof(IQueryConfiguration)) != null)
                .ToList();

            var parameters = exportedTypes
                .Where(x => x.IsClass && !x.IsAbstract && x.BaseType
                    .GetInterface(typeof(IParameter).Name) != null)
                .Select(type => (type, type.GetCustomAttribute<QueryParameterAttribute>()));

            var entitySettings = new List<IEntitySettings>();
            foreach (var entity in entityTypes)
            {
                //if (config.RoutesByAttribute)
                //{
                //    var attRoute = responseType?.GetCustomAttribute<GeneratedControllerAttribute>()?.Route;
                //}
                var responseType = types.GetFirstClassChild(typeof(EntityResponse), entity.Name);
                var commandRequest = types.GetFirstInterfaceChild(typeof(IModifyRequest), entity.Name);
                var queryRequest = types.GetFirstClassChild(typeof(GetRequest), entity.Name);

                var controller = config.EndpointSettingsCollection[entity];
                var controllerType = typeof(RapierController<,,>)
                    .MakeGenericType(
                        responseType,
                         queryRequest,
                        commandRequest);

                var authorizeEndpoints = new Dictionary<string, AuthorizationCategory>();
                foreach (var action in controller.ActionSettingsCollection)
                    authorizeEndpoints.Add(
                        $"{controllerType.FullName}.{action.ActionMethod}", 
                        action?.Authorize == AuthorizationCategory.None ?
                        controller.Authorize : action.Authorize);

                entitySettings.Add(new EntitySettings()
                {
                    EntityType = entity,
                    ResponseType = responseType,
                    QueryRequest =  queryRequest, // remove from types ?
                    CommandRequest = commandRequest,
                    QueryConfiguration = types.GetFirstInterfaceChild(typeof(IQueryConfiguration), entity.Name),
                    ControllerRoute = controller.Route,
                    ControllerName = controllerType.AssemblyQualifiedName,
                    Parameters = GetParameters(parameters, entity.Name),
                    Validator = GetValidator(exportedTypes, commandRequest),
                    AuthorizeableEndpoints = authorizeEndpoints

                });

            }
            config.ExtendedRepository = exportedTypes.FirstOrDefault(x => x.BaseType == typeof(Repository<,,>));
            config.EntitySettingsCollection = entitySettings;
            return config;
        }

        private static Type GetValidator(
            Type[] exportedTypes,
            Type commandRequest)
            => exportedTypes
            .FirstOrDefault(x => x.IsSubclassOf(typeof(AbstractValidator<>)
                .MakeGenericType(commandRequest))) ??
            typeof(DefaultValidation<>)
            .MakeGenericType(commandRequest);

        private static IDictionary<string, Type> GetParameters(
            IEnumerable<(Type, QueryParameterAttribute)> parameterTypes,
            string entityName)
            => new Dictionary<string, Type>().
                AddRange(parameterTypes
                    .Where(x => x.Item2.Entity == entityName)
                    .Select(x => new KeyValuePair<string, Type>(
                        x.Item2.Node, x.Item1)))
                .AddRange(
                (nameof(Entity.CreatedDate), typeof(CreatedDateParameter)),
                (nameof(Entity.UpdatedDate), typeof(UpdatedDateParameter)));

        public static void AddUriService(this IServiceCollection services)
        {
            services.AddScoped<IUriService>(provider =>
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri);
            });
        }
    }
}
