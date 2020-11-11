﻿using MediatR;
using Rapier.External.Models;
using Rapier.QueryDefinitions;
using Rapier.Server.Responses;

namespace Rapier.Server.QueryConfiguration
{
    public class BlogQuery : QueryReciever, IRequest<PagedResponse<BlogResponse>>
    {
        public BlogQuery(GetRequest query) : base(query)
        {
        }
    }
}