﻿using Rapier.External.Models;
using System;

namespace Rapier.Server.Requests
{
    public class GetPostsRequest : GetRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
    }
}
