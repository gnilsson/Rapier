using Rapier.External.Attributes;
using Rapier.External.Models;
using Rapier.QueryDefinitions.Parameters;
using Rapier.Server.Parameters;

namespace Rapier.Server.Requests
{
    public class GetBlogsRequest : GetRequest
    {
        [QueryParameterAttribute(typeof(StringParameter))]
        public string Title { get; set; }
    }
}
