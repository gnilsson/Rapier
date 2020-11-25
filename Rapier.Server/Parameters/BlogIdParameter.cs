using Rapier.External.Attributes;
using Rapier.QueryDefinitions.Parameters;
using Rapier.Server.Data;
using Rapier.Server.Requests;

namespace Rapier.Server.Parameters
{
    [QueryParameter(nameof(Post), nameof(GetPostsRequest.BlogId))]
    public class BlogIdParameter : ForeignIdParameter
    {
        public BlogIdParameter(string value)
        {
            base.Set(value);
            NavigationProperties = new[] { nameof(Post.BlogId) };
        }
    }
}
