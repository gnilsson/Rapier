using AutoMapper;
using Rapier.Configuration;
using Rapier.External.Handlers;
using Rapier.Internal.Repositories;
using Rapier.Server.Data;
using Rapier.Server.Models;
using Rapier.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.Server
{
    //public class OverrideHandler : GetHandler<Blog, BlogQuery, BlogResponse>
    //{
    //    public OverrideHandler(IRepositoryWrapper repositoryWrapper, IMapper mapping) : base(repositoryWrapper, mapping)
    //    {
    //    }

    //    public override async Task<BlogResponse> Handle(BlogQuery request, CancellationToken ct)
    //    {
    //        var abc = request;
    //        return new BlogResponse() { Title = "hej" };
    //    }
    //}
}
