using Rapier.Internal.Utility;
using System;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class ActionIntermediary
    {
        public ICollection<ActionDescription> ActionDescriptions { get; }
        public record ActionDescription(Type ResponseType, string Name, string Controller);
        public ActionIntermediary()
        {
            ActionDescriptions = new List<ActionDescription>();
        }

    }
}
