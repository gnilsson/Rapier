using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rapier.Configuration;
using Rapier.External.Extensions;
using Rapier.External.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
//using Serilog;
//using Serilog.Core;

namespace Rapier.External
{
   // [ApiExplorerSettings(GroupName = "Rapier Controller Collection")]
    public class RapierController<TResponse, TQuery, TCommand> :
        ControllerBase
        where TResponse : EntityResponse
        where TQuery : GetRequest
        where TCommand : IModifyRequest

    {
        private readonly IMediator _mediator;
        private readonly string _controllerName;

        public RapierController(IMediator mediator)
        {
            _mediator = mediator;
            _controllerName = this.GetType().Name;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TQuery request)
        {
            return await _mediator
                .Send(new GetQuery<TQuery, TResponse>(request))
                .ToOkResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TCommand request)
        {
            return await _mediator
                .Send(new CreateCommand<TCommand,TResponse>(request))
                .ToCreatedAtResult(nameof(GetById), _controllerName);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var a = await _mediator.Send(new GetByIdQuery<TResponse>(id));
            return a == null ? (IActionResult)NotFound() : Ok(a);
        }

        [HttpPut, Route("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TCommand request)
        {
            return null;
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return null;
        }

    }
}
