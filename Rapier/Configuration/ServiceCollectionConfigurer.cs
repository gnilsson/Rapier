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
            foreach (var entityType in entityTypes)
            {
                //if (config.RoutesByAttribute)
                //{
                //    var attRoute = responseType?.GetCustomAttribute<GeneratedControllerAttribute>()?.Route;
                //}
                var responseType = types.GetFirstClassChild(typeof(EntityResponse), entityType.Name);
                var commandRequestType = types.GetFirstInterfaceChild(typeof(IModifyRequest), entityType.Name);
                var queryRequestType = types.GetFirstClassChild(typeof(GetRequest), entityType.Name);

                var controllerEndpoint = config.EndpointSettingsCollection[entityType];
                var controllerType = typeof(RapierController<,,>)
                    .MakeGenericType(
                        responseType,
                         queryRequestType,
                        commandRequestType);


                entitySettings.Add(new EntitySettings()
                {
                    EntityType = entityType,
                    ResponseType = responseType,
                    QueryRequest =  queryRequestType,
                    CommandRequest = commandRequestType,
                    QueryConfiguration = types.GetFirstInterfaceChild(typeof(IQueryConfiguration), entityType.Name),
                    ControllerRoute = controllerEndpoint.Route,
                    ControllerName = controllerType.AssemblyQualifiedName,
                    Parameters = GetParameters(parameters, entityType.Name),
                    Validator = GetValidator(exportedTypes, commandRequestType),
                    AuthorizeableEndpoints = GetAuthorizeableEndpoints(controllerEndpoint,controllerType),
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

        private static IDictionary<string, AuthorizeableEndpoint> GetAuthorizeableEndpoints(
            ControllerEndpointSettings controllerEndpoint,
            Type controllerType)
        {
            var authorizeEndpoints = new Dictionary<string, AuthorizeableEndpoint>();
            foreach (var actionEndpoint in controllerEndpoint.ActionSettingsCollection) 
                authorizeEndpoints.Add(
                    $"{controllerType.FullName}.{actionEndpoint.ActionMethod}", new()
                    {
                        Category = actionEndpoint?.AuthorizeableEndpoint.Category == AuthorizationCategory.None ?
                        controllerEndpoint.AuthorizeableEndpoint.Category : actionEndpoint.AuthorizeableEndpoint.Category,

                        Policy = string.IsNullOrWhiteSpace(actionEndpoint.AuthorizeableEndpoint.Policy) ?
                        controllerEndpoint.AuthorizeableEndpoint.Policy : actionEndpoint.AuthorizeableEndpoint.Policy
                    });
            return authorizeEndpoints;
        }

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
