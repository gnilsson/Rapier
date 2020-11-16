using Rapier.External.Enums;
using Rapier.External.Models;

namespace Rapier.Configuration
{
    public abstract class EndpointBase
    {
        public AuthorizeableEndpoint AuthorizeableEndpoint { get; set; }

        public EndpointBase()
        {
            AuthorizeableEndpoint = new();
        }
        //public TEndpoint Authorize<TEndpoint>(
        //    AuthorizationCategory category,
        //    string policy = null) where TEndpoint : IEndpoint
        //{
        //    this.AuthorizeableEndpoint = new()
        //    {
        //        Category = category,
        //        Policy = policy
        //    };
        //    return this;
        //}
    }
}
