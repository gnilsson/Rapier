using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rapier.Configuration.Settings;
using Rapier.External;
using Rapier.External.PipelineBehaviours;
using Rapier.Internal;
using Rapier.Internal.Exceptions;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace Rapier.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRapierControllers(this IServiceCollection services, Action<RapierConfigurationOptions> options)
        {
            var config = options.Invoke();

            if (config.InterfaceDiscovery && config.AssemblyType != null)
                config.DiscoverInterfacesByEntityName();

            services.AddControllers(o =>
            {
                o.Conventions.Add(new GenericControllerRouteConvention(config));
            }).ConfigureApplicationPartManager(m => m.FeatureProviders
                .Add(new GenericTypeControllerFeatureProvider(config)))
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddSingleton(config);
        }
        public static void AddRapier(this IServiceCollection services)
        {
            services.BuildServiceProvider();
            var provider = services.BuildServiceProvider();

            var config = provider.GetRequiredService<RapierConfigurationOptions>();
            var entitySettings = new EntitySettingsContainer(config.EntitySettingsCollection);

            foreach (var handlerType in new HandlerTypesContainer())
                services.AddScoped(handlerType[0], handlerType[1]);

            var queryConfigurations =
                new Dictionary<string,
                ExpressionUtility.EmptyConstructorDelegate>();
            var repositories =
                new Dictionary<string,
                RepositoryConstructContainer>();
            var fullParameters =
                new Dictionary<string,
                IReadOnlyDictionary<string,
                ExpressionUtility.ConstructorDelegate>>();

            var generalQueryConfig = new QueryConfigurationGeneral();

            foreach (var setting in entitySettings)
            {
                var entityTypes = new EntityTypes(setting);

                var parameterDict = new Dictionary<string, ExpressionUtility.ConstructorDelegate>();
                foreach (var parameter in setting.Parameters)
                    parameterDict.Add(
                        parameter.Key,
                        ExpressionUtility.CreateConstructor(parameter.Value, typeof(string)));

                fullParameters.Add(setting.QueryRequest.Name, new ReadOnlyDictionary<string,
                    ExpressionUtility.ConstructorDelegate>(parameterDict));

                services.AddTransient(typeof(IValidator<>).MakeGenericType(setting.CommandRequest), setting.Validator);

                var properties = entityTypes
                    .GetType()
                    .GetProperties()
                    .Where(t => t.PropertyType.IsArray);
                foreach (var property in properties)
                    if (property.GetValue(entityTypes) is Type[] handler)
                        services.AddScoped(handler[0], handler[1]);

                var queryConfig = entityTypes.QueryConfiguration == null ? null :
                    ExpressionUtility.CreateEmptyConstructor(entityTypes.QueryConfiguration);
                var queryManagerType = typeof(QueryManager<>).MakeGenericType(setting.EntityType);
                var queryManagerConstructor = ExpressionUtility.CreateConstructor(
                    queryManagerType, typeof(IQueryConfiguration), typeof(QueryConfigurationGeneral));
                var queryManager = queryManagerConstructor(queryConfig(), generalQueryConfig);

                var repositoryConstructor = ExpressionUtility.CreateConstructor(
                    (config.ExtendedRepository ?? typeof(Repository<,,>))
                    .MakeGenericType(setting.EntityType, setting.ResponseType, config.ContextType),
                    config.ContextType,
                    typeof(IMapper),
                    typeof(QueryManager<>).MakeGenericType(setting.EntityType));

                repositories.Add(
                    setting.EntityType.Name,
                    new RepositoryConstructContainer(repositoryConstructor, queryManager));
            }

            var queryConfigContainer =
                new ReadOnlyDictionary<string,
                ExpressionUtility.EmptyConstructorDelegate>(queryConfigurations);
            var repositoryContainer =
                new ReadOnlyDictionary<string,
                RepositoryConstructContainer>(repositories);

            var providerItems = new RequestProviderItems
            {
                Parameters = new ReadOnlyDictionary<string, IReadOnlyDictionary<string,
                ExpressionUtility.ConstructorDelegate>>(fullParameters),
                PaginationSettings = config.PaginationSettings ?? new PaginationSettings(),
            };
            services.AddSingleton(providerItems);

            services.AddHttpContextAccessor();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProvideCommandBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProvideQueryBehaviour<,>));

            services.AddMediatR(typeof(RapierController<,,>));
            var mapper = new Mapping(entitySettings).ConfigureMapper();
            services.TryAddScoped(x => mapper);

            services.AddScoped(x =>
            {
                var context = x.GetRequiredService(config.ContextType);
                var type = context.GetType();
                var wrapperCtor = typeof(RepositoryWrapper<>)
                .MakeGenericType(type)
                .GetConstructor(new[] {
                    type,
                    typeof(IMapper),
                    typeof(IReadOnlyDictionary<string,
                    RepositoryConstructContainer>)});
                return (IRepositoryWrapper)wrapperCtor.Invoke(
                    new[] { context, mapper, repositoryContainer });
            });
            //services.Decorate<IRepositoryWrapper, CachedRepositoryWrapper>();

            services.AddUriService();
        }

        public static void AddRapierExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
