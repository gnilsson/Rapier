using Rapier.External;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rapier.Server.Data
{
    public class Post : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        public Blog Blog { get; set; }
        public Guid BlogId { get; set; }
    }
}
