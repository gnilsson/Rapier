using Rapier.External.Models;

namespace Rapier.Server.Requests
{
    public class GetPostsRequest : GetRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
