using MediatR;
using Rapier.CommandDefinitions;
using Rapier.Descriptive;
using Rapier.External.Attributes;
using Rapier.External.Enums;
using Rapier.External.Models;
using Rapier.Internal;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.PipelineBehaviours
{
    internal sealed class ProvideCommandBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandReciever<IModifyRequest>, IRequest<TResponse>, ICommand
    {
        private readonly IGeneralRepository _repository;
        public ProvideCommandBehaviour(IRepositoryWrapper repositoryWrapper)
        {
            _repository = repositoryWrapper.General;
        }
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            foreach (var property in request.Command.GetType().GetProperties())
                if (TryGetRequestValue(property, request.Command, out var keyPair))
                    if (property.TryGetAttribute<IdCollectionAttribute>(out var attribute))
                    {
                        var entities = await MethodFactory.GetManyAsync
                            .MakeGenericMethod(attribute.EntityType)
                            .InvokeAsync(
                            _repository,
                            new object[] { keyPair.Value, cancellationToken });

                        request.RequestPropertyValues.Add(keyPair.Key, entities);
                        request.RequestForeignEntities.Add(keyPair.Key, attribute.EntityType);
                        request.IncludeNavigation = string.Join(".", keyPair.Key);
                    }
                    else
                    {
                        request.RequestPropertyValues.Add(keyPair.Key, keyPair.Value);
                    }

            return await next();
        }

        private static bool TryGetRequestValue(PropertyInfo property, object data,
            out KeyValuePair<string, object> value)
        {
            var attribute = property.GetCustomAttribute<RequestParameterAttribute>();
            var propertyValue = property.GetValue(data);
            if (attribute?.Mode == RequestParameterMode.Hidden || propertyValue == null)
            {
                value = default;
                return false;
            }

            var propertyName = GetPropertyName(property, attribute);
            value = KeyValuePair.Create(propertyName, propertyValue);

            return propertyValue is not null ||
                  (propertyValue is int and 0) ||
                  (propertyValue is string and "");
        }

        private static string GetPropertyName(PropertyInfo property, RequestParameterAttribute parameterAttribute)
        {
            var isIdCollection = property.CustomAttributes
                .Any(x => x.AttributeType == typeof(IdCollectionAttribute));

            var propertyName = parameterAttribute == null && isIdCollection ?
                property.Name.Replace("Id", string.Empty) :
                parameterAttribute == null ?
                property.Name :
                parameterAttribute.EntityProperty;

            return propertyName;
        }
    }
}
