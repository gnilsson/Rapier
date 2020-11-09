using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapier.Internal.Repositories
{
    public class RepositoryConfiguration
    {
        public Type Context { get; set; }
        public DbContext DbContext { get; set; }
    }
}
