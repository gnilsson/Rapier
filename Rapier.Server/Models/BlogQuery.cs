using MediatR;
using Rapier.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Models
{
    public class BlogQuery : IRequest<BlogResponse>
    {
        public BlogQuery(object request)
        {
            var a = request;
        }
    }
}
