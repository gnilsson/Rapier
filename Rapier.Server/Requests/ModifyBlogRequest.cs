using Rapier.External.Models;
using Rapier.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rapier.Server.Requests
{
    public class ModifyBlogRequest : IModifyRequest
    {
        [Required]
        public string Title { get; set; }
        public BlogCategory? BlogCategory { get; set; }
    }
}
