using System;
using System.Collections.Generic;

namespace Rapier.External.Models
{
    public class IdentifierCollection<T> : List<Guid> where T : IEntity
    { }
}
