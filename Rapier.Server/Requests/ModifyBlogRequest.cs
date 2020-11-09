using Rapier.External.Models;
using Rapier.Server.Enums;

namespace Rapier.Server.Requests
{
    public class ModifyBlogRequest : IModifyRequest
    {
        public string Title { get; set; }
        public BlogCategory BlogCategory { get; set; }
    }
}
