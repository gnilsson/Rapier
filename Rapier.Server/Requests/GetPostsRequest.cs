using Rapier.External.Attributes;
using Rapier.External.Models;
using Rapier.QueryDefinitions.Parameters;
using System;

namespace Rapier.Server.Requests
{
    public class GetPostsRequest : GetRequest
    {
        [QueryParameter(typeof(StringParameter))]
        public string Title { get; set; }
        public string Content { get; set; }
        [QueryParameter(typeof(ForeignIdParameter))]
        public Guid BlogId { get; set; }
    }
}
