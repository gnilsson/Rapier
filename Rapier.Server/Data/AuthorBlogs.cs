using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Data
{
    public class AuthorBlogs
    {
        public Author Author { get; set; }
        public Guid AuthorId { get; set; }
        public Blog Blog { get; set; }
        public Guid BlogId { get; set; }

    }
}
