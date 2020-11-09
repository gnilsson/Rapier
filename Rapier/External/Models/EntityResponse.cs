using System;

namespace Rapier.External.Models
{
    public abstract class EntityResponse
    {
        public Guid Id { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}
