using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapier.External.Models
{
    public interface IModifyCommand<TCommand, TResponse>: IRequest<TResponse>
        where TCommand : IModifyRequest
    {
    }
}
