using Rapier.External.Models;
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
        public Type QueryRequestType { get; set; }
        public Type CommandRequestType { get; set; }
        public Type QueryConfiguration { get; set; }
        public IDictionary<string, Type> Parameters { get; set; }
        public Type Validator { get; set; }
        public IDictionary<string, AuthorizeableEndpoint> AuthorizeableEndpoints { get; set; }
        public Type[] RegisteredForeignEntities { get; set; }
    }
}
