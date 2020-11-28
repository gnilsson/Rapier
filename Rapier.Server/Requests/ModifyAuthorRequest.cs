using Rapier.External.Attributes;
using Rapier.External.Models;
using Rapier.Server.Data;
using Rapier.Server.Enums;
using System;
using System.Collections.Generic;

namespace Rapier.Server.Requests
{
    public class ModifyAuthorRequest : IModifyRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public ProfessionCategory? Profession { get; set; }
        [IdCollection(typeof(Blog))]
        public ICollection<Guid> BlogIds { get; set; }
    }
}
