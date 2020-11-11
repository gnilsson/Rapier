using Rapier.External.Enums;
using System;
using System.Collections.Generic;

namespace Rapier.Configuration.Settings
{
    public interface IEntitySettings
    {
        public Type EntityType { get; set; }
        public Type ResponseType { get; set; }
        public Type SimplifiedResponseType { get; set; }
        public string ControllerRoute { get; set; }
        public Type QueryRequest { get; set; }
        public Type CommandRequest { get; set; }
        public Type QueryConfiguration { get; set; }
        public IDictionary<string, Type> Parameters { get; set; }
        public Type Validator { get; set; }
        public IDictionary<string, AuthorizationCategory> AuthorizeableEndpoints { get; set; }
    }
}
