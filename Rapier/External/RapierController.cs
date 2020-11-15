using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rapier.Configuration;
using Rapier.External.Extensions;
using Rapier.External.Models;
using Rapier.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
//using Serilog;
//using Serilog.Core;

namespace Rapier.External
{
    //  [DynamicAuthorize("m")]
    public class RapierController<TResponse, TQuery, TCommand> :
        ControllerBase,
        IRapierController<TResponse, TQuery, TCommand>
        where TResponse : EntityResponse
        where TQuery : GetRequest
        where TCommand : IModifyRequest

    {
        private const string ID = "{id}";
        private readonly IMediator _mediator;
        public RapierController(IMediator mediator) => _mediator = mediator;

        [HttpGet, ActionName(nameof(Get))]
        public async Task<IActionResult> Get([FromQuery] TQuery request)
            => await _mediator
                .Send(new GetQuery<TQuery, TResponse>(request))
                .ToResult(Ok);

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TCommand request)
            => await _mediator
                .Send(new CreateCommand<TCommand, TResponse>(request))
                .ToResult(CreatedAtAction, nameof(GetById));

        [HttpGet, Route(ID)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var a = await _mediator.Send(new GetByIdQuery<TResponse>(id));
            return a == null ? (IActionResult)NotFound() : Ok(a);
        }

        [HttpPatch, Route(ID)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TCommand request)
            => await _mediator
                .Send(new UpdateCommand<TCommand, TResponse>(id, request))
                .ToResult(Ok);

        [HttpDelete, Route(ID)]
        public async Task<IActionResult> Delete(Guid id)
        {
            return null;
        }

    }
}
