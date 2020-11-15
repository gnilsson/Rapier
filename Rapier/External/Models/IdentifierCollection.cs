using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapier.External.Models
{
    public class IdentifierCollection<T> : List<Guid> where T : IEntity
    { }
}
