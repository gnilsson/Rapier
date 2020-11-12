using Rapier.External;
using Rapier.Server.Enums;
using System;
using System.Collections.Generic;

namespace Rapier.Server.Data
{
    public class Blog : IEntity
    {
        public Blog()
        {
            this.Posts = new HashSet<Post>();
            this.Authors = new HashSet<Author>();
        }
        public string Title { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Author> Authors { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
