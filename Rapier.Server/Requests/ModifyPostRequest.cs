using Rapier.External.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rapier.Server.Requests
{
    public class ModifyPostRequest : IModifyRequest
    {
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
    }
}
