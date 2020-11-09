using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapier.External.Models
{
    public class AddCommand<TResponse> :
        IRequest<TResponse>
    {

        public AddCommand(object request, Guid? createAtId = null, Type createAtType = null)
        {
            var a = request;
            CreateAtId = createAtId;
            CreateAtType = createAtType;
        }

        public Guid? CreateAtId { get; }
        public Type CreateAtType { get; set; }
    }
}
