using Rapier.Configuration;
using System.Collections.Generic;

namespace Rapier.External.Models
{
    public class DefaultQueryConfiguration : IQueryConfiguration
    {
        public ICollection<string[]> IncluderDetails => new List<string[]>();
    }
}
