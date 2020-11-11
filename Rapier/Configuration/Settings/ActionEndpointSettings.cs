using Rapier.External.Enums;

namespace Rapier.Configuration.Settings
{
    public class ActionEndpointSettings
    {
        public string ActionMethod { get; }
        public AuthorizationCategory Authorize { get; set; } = 0;
        public ActionEndpointSettings(string actionMethod)
        {
            ActionMethod = actionMethod;
        }
    }
}
