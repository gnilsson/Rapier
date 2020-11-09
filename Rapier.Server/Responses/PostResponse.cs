using Rapier.External.Models;
using System;

namespace Rapier.Server.Responses
{
    public class PostResponse : EntityResponse, IPostResponseSimplified
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
    }
}
