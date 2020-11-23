using MediatR;
using Rapier.CommandDefinitions;
using Rapier.External.Models;
using Rapier.Internal;
using Rapier.Internal.Repositories;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (property.TryGetPropertyValue(request.Command, out var keyPair))
                    if (property.PropertyType.BaseType == typeof(List<Guid>))
                    {
                        var entityType = property.PropertyType.GetGenericArguments()[0];

                        var entities = await MethodFactory.GetManyAsync
                            .MakeGenericMethod(entityType)
                            .InvokeAsync(
                            _repository,
                            new object[] { keyPair.Value, cancellationToken });

                        var colName = entityType.Name + 's'; // name registration
                        request.RequestPropertyValues.Add(colName, entities);
                        request.IncludeNavigation = string.Join(".", colName); // this is not right, right?
                    }
                    else
                    {
                        request.RequestPropertyValues.Add(keyPair.Key, keyPair.Value);
                    }

            return await next();
        }
    }
}
