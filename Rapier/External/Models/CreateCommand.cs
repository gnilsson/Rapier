using MediatR;
using Rapier.CommandDefinitions;

namespace Rapier.External.Models
{
    public class CreateCommand<TCommand, TResponse> :
        CommandReciever<TCommand>, IRequest<TResponse>
        where TCommand : IModifyRequest
    {
        public CreateCommand(TCommand request) : base(request)
        { }
    }
}
