using Microsoft.Extensions.Configuration;

namespace Rapier.Server.Config
{
    public abstract class SettingsBase<T> where T : SettingsBase<T>
    {
        public T Bind(IConfiguration configuration)
        {
            configuration.Bind(typeof(T).Name, (T)this);
            return (T)this;
        }
    }
}
