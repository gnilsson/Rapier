using Rapier.External.Models;
using Rapier.Server.Data;
using Rapier.Server.Enums;
using Rapier.Server.Requests.Identifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rapier.Server.Requests
{
    public class ModifyAuthorRequest : IModifyRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public ProfessionCategory? Profession { get; set; }
        public IdentifierCollection<Blog> BlogIds { get; set; }
    }
}
