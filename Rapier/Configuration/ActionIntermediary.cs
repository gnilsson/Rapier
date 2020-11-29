using System;
using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class ActionIntermediary
    {
        public ICollection<ActionDescription> ActionDescriptions { get; }

        public ActionIntermediary()
        {
            ActionDescriptions = new List<ActionDescription>();
        }

        public record ActionDescription(Type ResponseType, string Name, string Controller);
    }
}
