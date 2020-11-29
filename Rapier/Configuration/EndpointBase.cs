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
    }
}
