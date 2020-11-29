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
        public Type QueryConfigurationType { get; set; }
        public IDictionary<string, Type> ParameterTypes { get; set; }
        public Type ValidatorType { get; set; }
        public IDictionary<string, AuthorizeableEndpoint> AuthorizeableEndpoints { get; set; }
        public bool AutoExpandMembers { get; set; }
        public string[] ExplicitExpandedMembers { get; set; }
        public string[] ResponseMembers { get; set; }
    }
}
