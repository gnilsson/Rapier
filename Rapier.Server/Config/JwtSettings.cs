namespace Rapier.Server.Config
{
    internal class JwtSettings : SettingsBase<JwtSettings>
    {
        public string Secret { get; set; }
    }
}