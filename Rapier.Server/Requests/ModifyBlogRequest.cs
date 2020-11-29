using Rapier.External.Attributes;
using Rapier.External.Models;
using Rapier.Server.Data;
using Rapier.Server.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rapier.Server.Requests
{
    public class ModifyBlogRequest : IModifyRequest
    {
        [Required]
        public string Title { get; set; }
        public BlogCategory? BlogCategory { get; set; }
        [IdCollection(typeof(Author))]
        public ICollection<Guid> AuthorIds { get; set; }
    }
}
