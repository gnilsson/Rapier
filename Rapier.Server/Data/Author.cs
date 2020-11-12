using Rapier.External;
using Rapier.Server.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rapier.Server.Data
{
    public class Author : IEntity
    {
        public Author()
        {
            this.Blogs = new HashSet<Blog>();
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public ProfessionCategory Profession { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
