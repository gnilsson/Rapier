using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rapier.External;
using Rapier.External.Models;
using Rapier.Server.Data;
using Rapier.Server.Models;
using Rapier.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Controllers
{
    public class AppController :  ControllerBase //RapierController<Blog>
    {
        private readonly IMediator _mediator;

        public AppController(IMediator mediator) // : base(mediator)
        {
            _mediator = mediator;
        }


        //[HttpGet("/api/blogs")]
        //public async Task<IActionResult> Get([FromQuery] GetRequest request)
        //{
        //    var a = await Send(new GetQuery<BlogResponse>(request));
        //    return a == null ? (IActionResult)NotFound() : Ok(a);
        //}

        //[HttpPost("/api/blogs")]
        //public async Task<IActionResult> Post([FromBody] string text)
        //{
        //    var a = await Send(new CreateCommand<BlogResponse>(text));
        //    return a == null ? (IActionResult)NotFound() : Created("/api/blogs", a);
        //}
    }
}
