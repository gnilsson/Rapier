using Rapier.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapier.External.Models
{
    public class DefaultQueryConfiguration : IQueryConfiguration
    {
        public ICollection<string[]> IncluderDetails => new List<string[]>();
    }
}
