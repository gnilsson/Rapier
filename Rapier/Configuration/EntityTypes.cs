using MediatR;
using Rapier.CommandDefinitions;
using Rapier.Configuration.Settings;
using Rapier.External.Handlers;
using Rapier.External.Models;
using Rapier.External.PipelineBehaviours;
using System;

namespace Rapier.Configuration
{
    public class EntityTypes
    {
        public Type QueryConfiguration { get; }
        public Type[] GetHandler { get; }
        public Type[] CreateHandler { get; }
        public Type[] Modifier { get; }
        public Type[] CreateValidator { get; }
        public Type[] UpdateValidator { get; }
        public Type[] UpdateHandler { get; }
        public EntityTypes(IEntitySettings setting)
        {
            QueryConfiguration = setting.QueryConfiguration;

            GetHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(typeof(GetQuery<,>)
                .MakeGenericType(setting.QueryRequestType, setting.ResponseType),
                typeof(PagedResponse<>)
                .MakeGenericType(setting.ResponseType)),

                typeof(GetHandler<,,>)
                .MakeGenericType(setting.EntityType, typeof(GetQuery<,>)
                .MakeGenericType(setting.QueryRequestType, setting.ResponseType),
                setting.ResponseType)
            };

            CreateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType),

                 typeof(CreateHandler<,,>)
                .MakeGenericType(setting.EntityType, typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType)
            };

            UpdateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(typeof(UpdateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType),

                 typeof(UpdateHandler<,,>)
                .MakeGenericType(setting.EntityType, typeof(UpdateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType)
            };

            Modifier = new[]
            {
                typeof(IModifier<,>)
                .MakeGenericType(setting.EntityType,
                typeof(CommandReciever<>)
                .MakeGenericType(setting.CommandRequestType)),

                typeof(Modifier<,>)
                .MakeGenericType(setting.EntityType,
                typeof(CommandReciever<>)
                .MakeGenericType(setting.CommandRequestType))
            };

            CreateValidator = new[]
            {
                typeof(IPipelineBehavior<,>)
                .MakeGenericType(typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType),

                typeof(ValidationBehaviour<,,>)
                .MakeGenericType(typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType, setting.CommandRequestType)
            };

            UpdateValidator = new[]
            {
                typeof(IPipelineBehavior<,>)
                .MakeGenericType(typeof(UpdateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType),

                typeof(ValidationBehaviour<,,>)
                .MakeGenericType(typeof(UpdateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType),
                setting.ResponseType, setting.CommandRequestType)
            };
        }
    }
}
