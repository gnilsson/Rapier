using Rapier.QueryDefinitions.Parameters;
using System;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class EntitySetting : IEntitySetting
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
    }
}
