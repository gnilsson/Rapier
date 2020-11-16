using MediatR;
using System;

namespace Rapier.External.Models
{
    public class DeleteCommand //<TEntity> : 
        :IRequest<DeleteResponse> 
        //where TEntity : IEntity
    {
        public DeleteCommand(Guid entityId) => Id = entityId;
        public Guid Id { get; }
    }
}
