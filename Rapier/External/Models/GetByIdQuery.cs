using MediatR;
using System;

namespace Rapier.External.Models
{
    public class GetByIdQuery<TResponse> : IRequest<TResponse>
    {
        public GetByIdQuery(Guid id) => Id = id;
        public Guid Id { get; }
    }
}
