using Rapier.Configuration;
using Rapier.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.QueryConfiguration
{
    public class BlogQueryConfiguration : IQueryConfiguration
    {
        public ICollection<string[]> IncluderDetails { get; }
        public BlogQueryConfiguration()
        {
            IncluderDetails = new List<string[]>
            {
                new [] { nameof(Blog.Posts)}
            };
        }
    }
}
