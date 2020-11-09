using MediatR;
using Rapier.CommandDefinitions;
using System;

namespace Rapier.External.Models
{
    public class CreateCommand<TCommand, TResponse> : CommandReciever<TCommand>, IRequest<TResponse>  where TCommand : IModifyRequest
    //CommandReciever<TResponse, EmptyValidation>
    {
        public CreateCommand(TCommand request) 
            : base(request, Guid.NewGuid())
        {
    //        Req = request;
        }

    //    public object Req { get; set; }
    //
    }
}
