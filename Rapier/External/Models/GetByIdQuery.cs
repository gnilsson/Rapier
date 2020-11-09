using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapier.External.Models
{
    public class GetByIdQuery<TResponse> : IRequest<TResponse>
    {
        public GetByIdQuery(Guid id)
        {
            // base 64 ?
            var a = id;
        }
    }
}
