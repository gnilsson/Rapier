using Rapier.External.Models;

namespace Rapier.Configuration
{
    public interface IEndpoint
    {
        public AuthorizeableEndpoint AuthorizeableEndpoint { get; set; }
    }
}
