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
        public Type[] GetByIdHandler { get; set; }
        public Type[] DeleteHandler { get; set; }
        public EntityTypes(IEntitySettings setting)
        {
            QueryConfiguration = setting.QueryConfiguration;

            var getQuery = typeof(GetQuery<,>)
                .MakeGenericType(setting.QueryRequestType, setting.ResponseType);
            GetHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(getQuery, typeof(PagedResponse<>)
                .MakeGenericType(setting.ResponseType)),

                typeof(GetHandler<,,>)
                .MakeGenericType(setting.EntityType, getQuery, setting.ResponseType)
            };

            var createCommand = typeof(CreateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType);
            CreateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(createCommand,setting.ResponseType),

                 typeof(CreateHandler<,,>)
                .MakeGenericType(setting.EntityType, createCommand, setting.ResponseType)
            };

            var updateCommand = typeof(UpdateCommand<,>)
                .MakeGenericType(setting.CommandRequestType, setting.ResponseType);
            UpdateHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(updateCommand, setting.ResponseType),

                 typeof(UpdateHandler<,,>)
                .MakeGenericType(setting.EntityType, updateCommand,setting.ResponseType)
            };

            var getByIdQuery = typeof(GetByIdQuery<>)
                .MakeGenericType(setting.ResponseType);
            GetByIdHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(getByIdQuery, setting.ResponseType),

                typeof(GetByIdHandler<,,>)
                .MakeGenericType(setting.EntityType, getByIdQuery, setting.ResponseType)
            };

            var deleteCommand = typeof(DeleteCommand);
            DeleteHandler = new[]
            {
                typeof(IRequestHandler<,>)
                .MakeGenericType(deleteCommand, typeof(DeleteResponse)),

                typeof(DeleteHandler<,>)
                .MakeGenericType(setting.EntityType, deleteCommand)
            };

            var commandReciever = typeof(CommandReciever<>)
                .MakeGenericType(setting.CommandRequestType);
            Modifier = new[]
            {
                typeof(IModifier<,>)
                .MakeGenericType(setting.EntityType, commandReciever),

                typeof(Modifier<,>)
                .MakeGenericType(setting.EntityType, commandReciever)
            };

            CreateValidator = new[]
            {
                typeof(IPipelineBehavior<,>)
                .MakeGenericType(createCommand,setting.ResponseType),

                typeof(ValidationBehaviour<,,>)
                .MakeGenericType(createCommand,setting.ResponseType, setting.CommandRequestType)
            };

            UpdateValidator = new[]
            {
                typeof(IPipelineBehavior<,>)
                .MakeGenericType(updateCommand, setting.ResponseType),

                typeof(ValidationBehaviour<,,>)
                .MakeGenericType(updateCommand, setting.ResponseType, setting.CommandRequestType)
            };
        }
    }
}
