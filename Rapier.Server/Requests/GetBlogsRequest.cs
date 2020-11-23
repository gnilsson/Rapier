using Rapier.External.Models;

namespace Rapier.Server.Requests
{
    public class GetBlogsRequest : GetRequest
    {
        public string? Title { get; set; }
    }
}
