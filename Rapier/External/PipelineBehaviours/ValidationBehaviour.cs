using FluentValidation;
using MediatR;
using Rapier.CommandDefinitions;
using Rapier.Configuration;
using Rapier.External.Models;
using Rapier.Internal.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.PipelineBehaviours
{
    public class ValidationBehaviour<TRequest, TResponse, TCommand> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandReciever<TCommand> , IRequest<TResponse>
        where TCommand : IModifyRequest
    { 
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        public ValidationBehaviour(
            IEnumerable<IValidator<TCommand>> validators) =>
            (_validators) = (validators);

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TCommand>(request.Command);
            var failures = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
