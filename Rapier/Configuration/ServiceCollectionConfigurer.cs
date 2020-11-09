using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rapier.External;
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
                x.GetInterface(nameof(IQueryConfiguration)) != null ||
                typeof(AbstractValidator<>).IsAssignableFrom(x))
                .ToList();

            var parameters = exportedTypes
                .Where(x => x.IsClass && !x.IsAbstract && x.BaseType
                    .GetInterface(typeof(IParameter).Name) != null)
                .Select(type => (type, type.GetCustomAttribute<QueryParameterAttribute>()));

            var entitySettings = new List<IEntitySetting>();
            foreach (var entity in entityTypes)
            {
                var responseType = types.GetFirstClassChild(typeof(EntityResponse), entity.Name);
                var route = config.RoutesByAttribute ?
                    responseType?.GetCustomAttribute<GeneratedControllerAttribute>()?.Route ?? string.Empty :
                    config.Routes[entity] ?? string.Empty;
                var commandRequest = types.GetFirstInterfaceChild(typeof(IModifyRequest), entity.Name);

                entitySettings.Add(new EntitySetting
                {
                    EntityType = entity,
                    ResponseType = responseType,
                    QueryRequest = types.GetFirstClassChild(typeof(GetRequest), entity.Name), // remove from types ?
                    CommandRequest = commandRequest,
                    QueryConfiguration = types.GetFirstInterfaceChild(typeof(IQueryConfiguration), entity.Name),
                    ControllerRoute = route,
                    Parameters = GetParameters(parameters, entity.Name),
                    Validator = GetValidator(exportedTypes, commandRequest),

                });

            }
            config.ExtendedRepository = exportedTypes.FirstOrDefault(x => x.BaseType == typeof(Repository<,,>));
            config.EntitySettings = entitySettings;
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
                        x.Item2.Node, x.Item1)));

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
