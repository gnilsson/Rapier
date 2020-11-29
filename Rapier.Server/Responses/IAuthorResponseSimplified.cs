using Rapier.External.Models;

namespace Rapier.Server.Responses
{
    public interface IAuthorResponseSimplified : ISimplified
    {
        public string Firstname { get; set; }
    }
}
