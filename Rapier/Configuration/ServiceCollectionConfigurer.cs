﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rapier.Configuration.Settings;
using Rapier.External;
using Rapier.External.Attributes;
using Rapier.External.Enums;
using Rapier.External.Models;
using Rapier.Internal;
using Rapier.Internal.Exceptions;
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
            var exportedTypes = config.ContextType.Assembly.GetExportedTypes();

            CheckAttributes(exportedTypes);

            var entityTypes = GetEntityTypes(exportedTypes);
            var types = GetEntitiesCollectiveTypes(exportedTypes);
            var parameters = GetAllParameters(exportedTypes);
            var simplifiedResponseMembers = GetSimplifiedResponseMembers(types);

            var entitySettings = new List<IEntitySettings>();
            foreach (var entityType in entityTypes)
            {
                if (!config.EndpointSettingsCollection.TryGetValue(entityType, out var controllerEndpoint))
                    continue;

                var responseType = types.GetFirstClassChild(typeof(EntityResponse), entityType.Name);
                var commandRequestType = types.GetFirstInterfaceChild(typeof(IModifyRequest), entityType.Name);
                var queryRequestType = types.GetFirstClassChild(typeof(GetRequest), entityType.Name);

                var controllerType = typeof(RapierController<,,>)
                    .MakeGenericType(
                        responseType,
                         queryRequestType,
                        commandRequestType);

                entitySettings.Add(new EntitySettings()
                {
                    EntityType = entityType,
                    ResponseType = responseType,
                    QueryRequestType = queryRequestType,
                    CommandRequestType = commandRequestType,
                    QueryConfigurationType = types.GetFirstInterfaceChild(typeof(IQueryConfiguration), entityType.Name),
                    ControllerRoute = controllerEndpoint.Route,
                    ControllerName = controllerType.AssemblyQualifiedName,
                    ParameterTypes = GetParameters(parameters, entityType.Name),
                    ValidatorType = GetValidator(exportedTypes, commandRequestType),
                    AuthorizeableEndpoints = GetAuthorizeableEndpoints(controllerEndpoint, controllerType),
                    AutoExpandMembers = controllerEndpoint.AutoExpandMembers,
                    ExplicitExpandedMembers = controllerEndpoint.ExplicitExpandedMembers,
                    ResponseMembers = simplifiedResponseMembers[responseType]
                });
            }

            config.ExtendedRepositoryType = exportedTypes.FirstOrDefault(x => x.BaseType == typeof(Repository<,,>));
            config.EntitySettingsCollection = entitySettings;
            return config;
        }

        private static IDictionary<Type, string[]> GetSimplifiedResponseMembers(IEnumerable<Type> types)
        {
            var responses = types.Where(x => x.BaseType == typeof(EntityResponse));
            var dict = new Dictionary<Type, string[]>();

            foreach (var response in responses)
                dict.Add(response, response.GetProperties()
                    .Select(x => (x.Name, x.PropertyType.GetTypeInfo()))
                    .Where(x => x.Item2.GenericTypeArguments != Array.Empty<Type>())
                    .Select(x => (x, x.Item2.GetGenericArguments()[0]))
                    .Where(x => x.Item2.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ISimplified)))
                    .Select(x => x.x.Name)
                    .ToArray());

            return dict;
        }

        private static void CheckAttributes(Type[] exportedTypes)
        {
            var requests = exportedTypes
                .Where(x => x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IModifyRequest)));

            var idCollectionAttributes = requests
                .SelectMany(x => x.GetProperties())
                .Select(x => x.GetCustomAttribute<IdCollectionAttribute>())
                .Where(x => x != null);

            if (idCollectionAttributes != null)
                if (!idCollectionAttributes
                    .Any(x => x.EntityType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity))))
                    throw new InvalidConfigurationException("IdCollectionAttribute.EntityType must inherit IEntity.");
        }

        private static IEnumerable<(Type, QueryParameterAttribute)> GetAllParameters(Type[] exportedTypes)
            => exportedTypes
                .Where(x => x.IsClass && !x.IsAbstract && x.BaseType
                    .GetInterface(typeof(IParameter).Name) != null)
                .Select(type => (type, type.GetCustomAttribute<QueryParameterAttribute>()));

        private static IEnumerable<Type> GetEntitiesCollectiveTypes(Type[] exportedTypes)
            => exportedTypes
                .Where(x =>
                    !x.IsAbstract &&
                    x.BaseType == typeof(EntityResponse) ||
                    x.BaseType == typeof(GetRequest) ||
                    x.GetInterface(nameof(IModifyRequest)) != null ||
                    x.GetInterface(nameof(IQueryConfiguration)) != null);

        private static IOrderedEnumerable<Type> GetEntityTypes(Type[] exportedTypes)
            => exportedTypes
                .Where(x =>
                    !x.IsAbstract &&
                    !x.IsInterface &&
                    x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity)))
                .OrderByDescending(x => x.Name.Length);

        private static Type GetValidator(Type[] exportedTypes, Type commandRequest)
            => exportedTypes
                .FirstOrDefault(x => x.IsSubclassOf(typeof(AbstractValidator<>)
                    .MakeGenericType(commandRequest))) ??
                typeof(DefaultValidation<>).MakeGenericType(commandRequest);

        private static IDictionary<string, Type> GetParameters(
            IEnumerable<(Type, QueryParameterAttribute)> parameterTypes,
            string entityName)
            => new Dictionary<string, Type>()
                .AddRange(parameterTypes
                    .Where(x => x.Item2.Entity == entityName)
                    .Select(x => new KeyValuePair<string, Type>(
                        x.Item2.Node, x.Item1)))
                .AddRange(
                (nameof(IEntity.CreatedDate), typeof(CreatedDateParameter)),
                (nameof(IEntity.UpdatedDate), typeof(UpdatedDateParameter)));

        private static IDictionary<string, AuthorizeableEndpoint> GetAuthorizeableEndpoints(
            ControllerEndpointSettings controllerEndpoint, Type controllerType)
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
