using Rapier.External.Models;
using System.Collections.Generic;

namespace Rapier.Server.Responses
{
    public class BlogResponse : EntityResponse, IBlogResponseSimplified
    {
        public string Title { get; set; }
        public IEnumerable<IPostResponseSimplified> Posts { get; set; }
        public string BlogCategory { get; set; }
        public IEnumerable<IAuthorResponseSimplified> Authors { get; set; }
    }
}
