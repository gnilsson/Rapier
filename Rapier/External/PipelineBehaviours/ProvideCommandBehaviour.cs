using MediatR;
using Rapier.CommandDefinitions;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.PipelineBehaviours
{
    public class ProvideCommandBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandReciever<IModifyRequest>, IRequest<TResponse>, ICommand
    {
        public async Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            request.IgnoredProperties = new string[]
            { 
                nameof(Entity.CreatedDate), 
                nameof(Entity.UpdatedDate) 
            };
            request.RequestPropertyValues = new Dictionary<string, (object, Type)>
            {
                { nameof(ICommand.Id), (request.Id,typeof(Guid)) }
            };

            var properties = request.Command
                .GetType()
                .GetProperties()
                .Where(x => !request.IgnoredProperties.Contains(x.Name));

            foreach (var prop in properties)
                if (prop.TryGetPropertyValue(request.Command, out var value))
                    request.RequestPropertyValues.Add(
                        value.Key, (value.Value, value.Value.GetType()));

            return await next();
        }
    }
}
