using Rapier.External.Models;
using Rapier.Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Responses
{
    public class AuthorResponse : EntityResponse
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Profession { get; set; }
        public IEnumerable<IBlogResponseSimplified> Blogs { get; set; }
    }
}
