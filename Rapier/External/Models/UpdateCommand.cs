using MediatR;
using Rapier.CommandDefinitions;
using System;

namespace Rapier.External.Models
{
    public class UpdateCommand<TCommand, TResponse> :
        CommandReciever<TCommand>, IRequest<TResponse>
        where TCommand : IModifyRequest
    {
        public UpdateCommand(Guid id, TCommand request)
            : base(request) => (Id) = (id);
    }
}
