using Rapier.External.Attributes;
using Rapier.External.Models;
using Rapier.QueryDefinitions.Parameters;

namespace Rapier.Server.Requests
{
    public class GetBlogsRequest : GetRequest
    {
        [QueryParameter(typeof(StringParameter))]
        public string Title { get; set; }
    }
}
