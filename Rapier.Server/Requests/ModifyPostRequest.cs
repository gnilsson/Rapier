using Rapier.External.Models;
using System;

namespace Rapier.Server.Requests
{
    public class ModifyPostRequest : IModifyRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
    }
}
