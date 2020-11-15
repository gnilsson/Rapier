using Rapier.External.Models;
using Rapier.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Requests.Identifiers
{
    public class BlogIdentifierCollection : List<Guid>, IIdentifierCollection<Blog>
    { }
}
