using Rapier.External;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rapier.Server.Data
{
    public class Post : IEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        [Required]
        public Blog Blog { get; set; }
        public Guid BlogId { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
