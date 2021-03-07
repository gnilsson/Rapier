using FluentValidation;
using Rapier.Configuration.Settings;
using Rapier.Descriptive;
using Rapier.External;
using Rapier.External.Attributes;
using Rapier.External.Enums;
using Rapier.External.Models;
using Rapier.External.Models.Records;
using Rapier.Internal;
using Rapier.Internal.Exceptions;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions.Parameters;
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
            var parameters = GetAllParameters(types);
            var fieldDescriptions = GetFieldDescriptions(types);

            var entitySettings = new List<IEntitySettings>();
            foreach (var entityType in entityTypes)
            {
                if (!config.EndpointSettingsCollection.TryGetValue(entityType, out var controllerEndpoint))
                    continue;

                PopulateEntitySettings(exportedTypes, types, parameters, fieldDescriptions, entitySettings, entityType, controllerEndpoint);
            }

            config.ExtendedRepositoryType = exportedTypes.FirstOrDefault(x => x.BaseType == typeof(Repository<,,>));
            config.EntitySettingsCollection = entitySettings;
            return config;
        }

        private static void PopulateEntitySettings(Type[] exportedTypes, IEnumerable<Type> types, 
            IDictionary<Type, IEnumerable<ParameterConfigurationDescription>> parameters, IDictionary<Type, IEnumerable<FieldDescription>> fieldDescriptions, 
            List<IEntitySettings> entitySettings, Type entityType, ControllerEndpointSettings controllerEndpoint)
        {
            var responseType = types.GetFirstClassChild(typeof(EntityResponse), entityType.Name);
            var queryRequestType = types.GetFirstClassChild(typeof(GetRequest), entityType.Name);
            var commandRequestType = types.GetFirstInterfaceChild(typeof(IModifyRequest), entityType.Name);

            var controllerType = typeof(RapierController<,,>)
                .MakeGenericType(
                    responseType,
                     queryRequestType,
                    commandRequestType);

            entitySettings.Add(new EntitySettings
            {
                EntityType = entityType,
                ResponseType = responseType,
                QueryRequestType = queryRequestType,
                CommandRequestType = commandRequestType,
                QueryConfigurationType = types.GetFirstInterfaceChild(typeof(IQueryConfiguration), entityType.Name),
                ControllerRoute = controllerEndpoint.Route,
                ControllerName = controllerType.AssemblyQualifiedName,
                ParameterConfigurations = parameters.FirstOrDefault(x => x.Key == queryRequestType).Value,
                ValidatorType = GetValidator(exportedTypes, commandRequestType),
                AuthorizeableEndpoints = GetAuthorizeableEndpoints(controllerEndpoint, controllerType),
                AutoExpandMembers = controllerEndpoint.AutoExpandMembers,
                ExplicitExpandedMembers = controllerEndpoint.ExplicitExpandedMembers,
                FieldDescriptions = fieldDescriptions.FirstOrDefault(x => x.Key == responseType)
            });
        }

        private static IDictionary<Type, IEnumerable<FieldDescription>> GetFieldDescriptions(IEnumerable<Type> types)
        {
            var responses = types.Where(x => x.BaseType == typeof(EntityResponse));
            var fieldCollection = new Dictionary<Type, IEnumerable<FieldDescription>>();
            foreach (var response in responses)
            {
                var properties = response.GetProperties()
                    .Select(x => (x.Name, x.PropertyType.GetTypeInfo()));

                var fields = properties
                    .Where(x => x.Item2.GenericTypeArguments != Array.Empty<Type>())
                    .Select(x => (x, x.Item2.GetGenericArguments()[0]))
                    .Where(x => x.Item2.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ISimplified)))
                    .Select(x => new FieldDescription(x.x.Name, FieldCategory.Relational))
                    .ToList();

                var defaultFields = properties
                    .Where(y => fields.All(x => !y.Name.Equals(x.Name)))
                    .Select(x => new FieldDescription(x.Name, FieldCategory.Default));

                fields.AddRange(defaultFields);
                fieldCollection.Add(response, fields);
            }

            return fieldCollection;
        }

        private static void CheckAttributes(Type[] exportedTypes)
        {
            var requests = exportedTypes
                .Where(x => x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IModifyRequest)));

            var idCollectionAttributes = requests
                .SelectMany(x => x.GetProperties())
                .Select(x => x.GetCustomAttribute<IdCollectionAttribute>())
                .Where(x => x != null);

            // check parameter attri

            if (idCollectionAttributes != null)
                if (!idCollectionAttributes
                    .Any(x => x.EntityType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity))))
                    throw new InvalidConfigurationException(ErrorMessage.Configuration.IdCollectionAttribute);
        }

        private static IDictionary<Type, IEnumerable<ParameterConfigurationDescription>> GetAllParameters(IEnumerable<Type> collectedTypes)
        {
            var requestTypes = collectedTypes.Where(x => x.BaseType == typeof(GetRequest));

            var parameterDescriptionsCollection = new Dictionary<Type, IEnumerable<ParameterConfigurationDescription>>();
            foreach (var requestType in requestTypes)
            {
                var parameterDescriptions = new List<ParameterConfigurationDescription>()
                {
                    new(typeof(CreatedDateParameter), nameof(GetRequest.CreatedDate), new[] { nameof(IEntity.CreatedDate) }),
                    new(typeof(UpdatedDateParameter), nameof(GetRequest.UpdatedDate), new[] { nameof(IEntity.UpdatedDate) })
                };

                CollectParameterDescriptions(
                    parameterDescriptionsCollection, requestType, parameterDescriptions);
            }

            return parameterDescriptionsCollection;
        }

        private static void CollectParameterDescriptions(
            Dictionary<Type, IEnumerable<ParameterConfigurationDescription>> parameterDescriptionsCollection,
            Type requestType, List<ParameterConfigurationDescription> parameterDescriptions)
        {
            foreach (var property in requestType.GetProperties())
            {
                var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
                if (attribute != null)
                    parameterDescriptions.Add(
                        new ParameterConfigurationDescription(
                            attribute.ParameterType, property.Name, attribute.NavigationNodes ?? new[] { property.Name }));
            }

            parameterDescriptionsCollection.Add(requestType, parameterDescriptions);
        }

        private static IEnumerable<Type> GetEntitiesCollectiveTypes(Type[] exportedTypes)
        {
            return exportedTypes.Where(x =>
                !x.IsAbstract &&
                x.BaseType == typeof(EntityResponse) ||
                x.BaseType == typeof(GetRequest) ||
                x.GetInterface(nameof(IModifyRequest)) != null ||
                x.GetInterface(nameof(IQueryConfiguration)) != null); // move this last piece?
        }

        private static IOrderedEnumerable<Type> GetEntityTypes(Type[] exportedTypes)
        {
            return exportedTypes.Where(x =>
                 !x.IsAbstract &&
                 !x.IsInterface &&
                 x.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity)))
                .OrderByDescending(x => x.Name.Length);
        }

        private static Type GetValidator(Type[] exportedTypes, Type commandRequest)
        {
            return exportedTypes
                .FirstOrDefault(x => x.IsSubclassOf(typeof(AbstractValidator<>)
                    .MakeGenericType(commandRequest))) ??
                typeof(DefaultValidation<>).MakeGenericType(commandRequest);
        }

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
    }
}
