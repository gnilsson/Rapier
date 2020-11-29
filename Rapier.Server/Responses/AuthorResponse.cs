using Rapier.External.Models;
using System.Collections.Generic;

namespace Rapier.Server.Responses
{
    public class AuthorResponse : EntityResponse, IAuthorResponseSimplified
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Profession { get; set; }
        public IEnumerable<IBlogResponseSimplified> Blogs { get; set; }
    }
}
