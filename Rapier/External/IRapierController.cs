using Microsoft.AspNetCore.Mvc;
using Rapier.External.Models;
using System;
using System.Threading.Tasks;

namespace Rapier.External
{
    public interface IRapierController<in TQuery, in TCommand> : IExtendedController
        where TQuery : GetRequest
        where TCommand : IModifyRequest
    {
        Task<IActionResult> Get([FromQuery] TQuery request);
        Task<IActionResult> Create([FromBody] TCommand request);
        Task<IActionResult> GetById(Guid id);
        Task<IActionResult> Update(Guid id, [FromBody] TCommand request);
        Task<IActionResult> Delete(Guid id);
    }
}
