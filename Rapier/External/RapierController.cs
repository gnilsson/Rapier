using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rapier.Configuration;
using Rapier.External.Extensions;
using Rapier.External.Models;
using System;
using System.Threading.Tasks;

namespace Rapier.External
{
    public class RapierController<TResponse, TQuery, TCommand> :
        ControllerBase,
        IRapierController<TQuery, TCommand>
        where TResponse : EntityResponse
        where TQuery : GetRequest
        where TCommand : IModifyRequest

    {
        private const string ID = "{id}";
        private readonly IMediator _mediator;
        public RapierController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
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
            => await _mediator
                .Send(new GetByIdQuery<TResponse>(id))
                .ToResult(Ok);

        [HttpPatch, Route(ID)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TCommand request)
            => await _mediator
                .Send(new UpdateCommand<TCommand, TResponse>(id, request))
                .ToResult(Ok);

        [HttpDelete, Route(ID)]
        public async Task<IActionResult> Delete(Guid id)
            => await _mediator
                .Send(new DeleteCommand(id))
                .ToResult(Ok);
    }
}
