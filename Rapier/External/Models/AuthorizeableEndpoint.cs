using Rapier.External.Enums;

namespace Rapier.External.Models
{
    public class AuthorizeableEndpoint
    {
        public AuthorizationCategory Category { get; set; } = 0;
        public string Policy { get; set; }
    }
}
