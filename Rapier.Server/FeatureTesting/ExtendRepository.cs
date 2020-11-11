using AutoMapper;
using Rapier.Internal.Repositories;
using Rapier.QueryDefinitions;
using Rapier.Server.Data;
using Rapier.Server.Responses;

namespace Rapier.Server.FeatureTesting
{
    public class ExtendRepository : Repository<Blog, BlogResponse, RapierDbContext>, IRepository<Blog, BlogResponse>
    {
        public ExtendRepository(RapierDbContext context, IMapper mapper, QueryManager<Blog> queryManager) : 
            base(context, mapper, queryManager)
        {
        }

    }
}
