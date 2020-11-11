using AutoMapper;
using Rapier.External.Handlers;
using Rapier.External.Models;
using Rapier.Internal.Repositories;
using Rapier.Server.Data;
using Rapier.Server.QueryConfiguration;
using Rapier.Server.Responses;
using Rapier.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Rapier.Server.FeatureTesting
{
    public class OverrideHandler : GetHandler<Blog, BlogQuery, BlogResponse>
    {
        public OverrideHandler(IRepositoryWrapper repositoryWrapper, IMapper mapping, IUriService uriService) : base(repositoryWrapper, mapping, uriService)
        {
        }

        public override async Task<PagedResponse<BlogResponse>> Handle(BlogQuery request, CancellationToken ct)
        {
            //_uriService.
            var abc = request;
            return await base.Handle(request, ct);
        }
    }
}
