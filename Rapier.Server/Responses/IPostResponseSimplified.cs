using Rapier.External.Models;
using System;

namespace Rapier.Server.Responses
{
    public interface IPostResponseSimplified : ISimplified
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}