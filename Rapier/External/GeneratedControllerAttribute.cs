using System;

namespace Rapier.External
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GeneratedControllerAttribute : Attribute
    {
        public GeneratedControllerAttribute(string route) 
            => Route = route;

        public string Route { get; set; }
    }
}
