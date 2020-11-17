using MediatR;
using System;

namespace Rapier.External.Models
{
    public class DeleteCommand : IRequest<DeleteResponse>
    {
        public DeleteCommand(Guid entityId) => Id = entityId;
        public Guid Id { get; }
    }
}
