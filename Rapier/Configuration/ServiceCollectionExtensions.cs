using AutoMapper;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rapier.Configuration.Settings;
using Rapier.External;
using Rapier.External.Models;
using Rapier.External.Models.Records;
using Rapier.External.PipelineBehaviours;
using Rapier.Internal;
using Rapier.Internal.Exceptions;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using Rapier.QueryDefinitions;
using Rapier.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rapier.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRapierControllers(this IServiceCollection services, Action<RapierConfigurationOptions> options)
        {
            var config = options.Invoke();

            if (config.InterfaceDiscovery && config.ContextType != null)
                config.DiscoverInterfacesByEntityName(); // concealed config?

            services.AddSingleton(config);
            if (!config.GeneratedControllers)
                return;

            var actionIntermediary = new ActionIntermediary();

            services.AddControllers(o =>
            {
                o.Conventions.Add(new GenericControllerRouteConvention(
                    config.EntitySettingsCollection, actionIntermediary));
            }).ConfigureApplicationPartManager(m => m.FeatureProviders
                .Add(new GenericTypeControllerFeatureProvider(config.EntitySettingsCollection)))
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSingleton(actionIntermediary);
        }

        public static void AddRapier(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            var config = provider.GetRequiredService<RapierConfigurationOptions>();
            var entitySettings = new EntitySettingsContainer(config.EntitySettingsCollection);

            foreach (var handlerType in new HandlerTypesContainer())
                services.AddScoped(handlerType[0], handlerType[1]);

            var repositoryShells = new Dictionary<string, RepositoryShell>();
            var parameterShells = new Dictionary<string, IReadOnlyDictionary<string, QueryParameterShell>>();

            //     var expandeableMembers = new Dictionary<Type, string[]>();


            foreach (var setting in entitySettings)
            {
                parameterShells.Add(setting.QueryRequestType.Name, CreateEntityParameterShells(setting));

                services.AddTransient(typeof(IValidator<>).MakeGenericType(setting.CommandRequestType), setting.ValidatorType);

                services.AddEntityHandlers(setting);

                var queryManager = CreateQueryManager(setting);

                var repositoryConstructor = CreateRepositoryConstructor(
                    setting, config.ContextType, config.ExtendedRepositoryType);

                repositoryShells.Add(
                    setting.EntityType.Name,
                    new RepositoryShell(repositoryConstructor, queryManager));

                //expandeableMembers.Add(setting.ResponseType, setting.ResponseMembers);
            }

            services.AddSingleton(new SemanticsDefiner(
                provider.GetRequiredService<IActionDescriptorCollectionProvider>(),
                provider.GetRequiredService<ActionIntermediary>(),
                new Dictionary<Type, IEnumerable<FieldDescription>>(
                    entitySettings.Select(x => x.FieldDescriptions))));

            services.AddSingleton(new RequestProviderItems
            {
                Parameters = new ReadOnlyDictionary<string, IReadOnlyDictionary<string, QueryParameterShell>>(parameterShells),
                PaginationSettings = config.PaginationSettings ?? new PaginationSettings(),
            });

            services.AddHttpContextAccessor();

            services.AddPipelineBehaviors();

            services.AddMediatR(typeof(RapierController<,,>));

            var mapper = new Mapping(entitySettings).ConfigureMapper();
            services.TryAddScoped(x => mapper);

            services.AddRepositoryWrapper(config.ContextType, mapper, repositoryShells);

            services.AddUriService();
        }

        public static void AddRapierExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        private static IReadOnlyDictionary<string, QueryParameterShell> CreateEntityParameterShells(IEntitySettings setting)
        {
            var parameterDict = new Dictionary<string, QueryParameterShell>();

            foreach (var parameter in setting.ParameterConfigurations)
            {
                parameterDict.Add(parameter.PropertyName,
                new QueryParameterShell(ExpressionUtility.CreateConstructor(
                        parameter.ParameterType, typeof(string), typeof(string[])), parameter.NavigationNodes));
            }

            return new ReadOnlyDictionary<string, QueryParameterShell>(parameterDict);
        }

        private static void AddEntityHandlers(this IServiceCollection services, IEntitySettings setting)
        {
            var entityTypes = new EntityTypes(setting);
            var properties = entityTypes
                    .GetType()
                    .GetProperties()
                    .Where(t => t.PropertyType.IsArray);

            foreach (var property in properties)
                if (property.GetValue(entityTypes) is Type[] handler)
                    services.AddScoped(handler[0], handler[1]);
        }

        private static object CreateQueryManager(IEntitySettings setting)
        {
            var queryConfigConstructor = ExpressionUtility.CreateConstructor(
                typeof(QueryConfiguration<>).MakeGenericType(setting.EntityType),
                typeof(ICollection<string>));

            var queryManagerConstructor = ExpressionUtility.CreateConstructor(
                typeof(QueryManager<>).MakeGenericType(setting.EntityType),
                typeof(IQueryConfiguration));

            var queryConfig = queryConfigConstructor(setting.ExplicitExpandedMembers?.ToList());
            var queryManager = queryManagerConstructor(queryConfig);
            return queryManager;
        }

        private static ExpressionUtility.ConstructorDelegate CreateRepositoryConstructor(
            IEntitySettings setting, Type contextType, Type extendedRepositoryType)
        {
            var repositoryConstructor = ExpressionUtility.CreateConstructor(
                    (extendedRepositoryType ?? typeof(Repository<,,>))
                    .MakeGenericType(setting.EntityType, setting.ResponseType, contextType),
                    contextType,
                    typeof(IMapper),
                    typeof(QueryManager<>).MakeGenericType(setting.EntityType));
            return repositoryConstructor;
        }

        private static void AddPipelineBehaviors(this IServiceCollection services)
        {
            //  services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProvideCommandBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProvideQueryBehaviour<,>));
        }

        private static void AddRepositoryWrapper(this IServiceCollection services, Type dbContextType,
            IMapper mapper, Dictionary<string, RepositoryShell> repositories)
        {
            services.AddScoped(x =>
            {
                var context = x.GetRequiredService(dbContextType);
                var wrapperCtor = typeof(RepositoryWrapper<>)
                .MakeGenericType(dbContextType)
                .GetConstructor(new[] {
                    dbContextType,
                    typeof(IMapper),
                    typeof(IReadOnlyDictionary<string,
                    RepositoryShell>)});
                return (IRepositoryWrapper)wrapperCtor.Invoke(
                    new[] { context, mapper,
                        new ReadOnlyDictionary<string, RepositoryShell>(repositories) });
            });
            //services.Decorate<IRepositoryWrapper, CachedRepositoryWrapper>();
        }

        private static void AddUriService(this IServiceCollection services)
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
