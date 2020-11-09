using Rapier.External.Models;
using System;

namespace Rapier.Server.Responses
{
    public interface IBlogResponseSimplified : ISimplified
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
