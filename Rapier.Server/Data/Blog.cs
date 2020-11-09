using Microsoft.EntityFrameworkCore;
using Rapier.External;
using Rapier.Internal;
using Rapier.Server.Enums;
using System.Collections.Generic;

namespace Rapier.Server.Data
{
    public class Blog : Entity
    {
        public Blog()
        {
            this.Posts = new HashSet<Post>();
        }
        public ICollection<Post> Posts { get; set; }
        public string Title { get; set; }
        public BlogCategory BlogCategory { get; set; }
    }
}
