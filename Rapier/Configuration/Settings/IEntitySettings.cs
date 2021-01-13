using Rapier.External.Models;
using Rapier.External.Models.Records;
using System;
using System.Collections.Generic;

namespace Rapier.Configuration.Settings
{
    public interface IEntitySettings
    {
        public Type EntityType { get; init; }
        public Type ResponseType { get; init; }
        public Type SimplifiedResponseType { get; init; }
        public string ControllerRoute { get; init; }
        public Type QueryRequestType { get; init; }
        public Type CommandRequestType { get; init; }
        public Type QueryConfigurationType { get; init; }
   //     public IDictionary<string, Type> ParameterTypes { get; init; }
        public IEnumerable<ParameterConfigurationDescription> ParameterConfigurations { get; init; }
        public Type ValidatorType { get; init; }
        public IDictionary<string, AuthorizeableEndpoint> AuthorizeableEndpoints { get; init; }
        public bool AutoExpandMembers { get; init; }
        public string[] ExplicitExpandedMembers { get; init; }
        public string[] ResponseMembers { get; init; }
        public KeyValuePair<Type, IEnumerable<FieldDescription>> FieldDescriptions { get; init; }
    }
}
