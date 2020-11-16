using FluentValidation;
using MediatR;
using Rapier.CommandDefinitions;
using Rapier.Exceptions;
using Rapier.External.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.External.PipelineBehaviours
{
    internal sealed class ValidationBehaviour<TRequest, TResponse, TCommand> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandReciever<TCommand>, IRequest<TResponse>
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
            if (request.Command is null) throw new BadRequestException("Invalid input");

            var context = new ValidationContext<TCommand>(request.Command);
            var failures = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();

            if (failures.Any()) throw new ValidationException(failures);

            return await next();
        }
    }
}
