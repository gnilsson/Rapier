namespace Rapier.Configuration.Settings
{
    public class ActionEndpointSettings : EndpointBase, IEndpoint
    {
        public string ActionMethod { get; }
        public ActionEndpointSettings(string actionMethod)
        {
            ActionMethod = actionMethod;
        }
    }
}
