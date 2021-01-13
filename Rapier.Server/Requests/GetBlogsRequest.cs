using Rapier.External.Attributes;
using Rapier.External.Models;
using Rapier.Server.Parameters;

namespace Rapier.Server.Requests
{
    public class GetBlogsRequest : GetRequest
    {
        [QueryParameterAttribteNew(typeof(BlogTitleParameter))]
        public string Title { get; set; }
    }
}
