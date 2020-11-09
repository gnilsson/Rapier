using MediatR;
using Rapier.CommandDefinitions;
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
        public Type[] Validator { get; }
        public EntityTypes(IEntitySetting setting)
        {
            QueryConfiguration = setting.QueryConfiguration;

            GetHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(typeof(GetQuery<,>)
                .MakeGenericType(setting.QueryRequest, setting.ResponseType),
                typeof(PagedResponse<>)
                .MakeGenericType(setting.ResponseType)),

                typeof(GetHandler<,,>)
                .MakeGenericType(setting.EntityType, typeof(GetQuery<,>)
                .MakeGenericType(setting.QueryRequest, setting.ResponseType),
                setting.ResponseType)
            };

            CreateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequest, setting.ResponseType),
                setting.ResponseType),

                 typeof(CreateHandler<,,>)
                .MakeGenericType(setting.EntityType, typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequest, setting.ResponseType),
                setting.ResponseType)
            };

            Modifier = new[]
            {
                typeof(IModifier<,>)
                .MakeGenericType(setting.EntityType,
                typeof(CommandReciever<>)
                .MakeGenericType(setting.CommandRequest)),

                typeof(Modifier<,>)
                .MakeGenericType(setting.EntityType,
                typeof(CommandReciever<>)
                .MakeGenericType(setting.CommandRequest))
            };

            Validator = new[]
            {
                typeof(IPipelineBehavior<,>)
                .MakeGenericType(typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequest, setting.ResponseType),
                setting.ResponseType),

                typeof(ValidationBehaviour<,,>)
                .MakeGenericType(typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequest, setting.ResponseType),
                setting.ResponseType, setting.CommandRequest)
            };
        }
    }
}
