using Rapier.External.Enums;
using Rapier.External.Models;
using System;

namespace Rapier.Configuration.Settings
{
    public class ActionEndpointSettings
    {
        public string ActionMethod { get; }
        public AuthorizeableEndpoint AuthorizeableEndpoint { get; set; }
        public ActionEndpointSettings(string actionMethod)
        {
            ActionMethod = actionMethod;
            AuthorizeableEndpoint = new();
        }
    }
}
