using System.Collections.Generic;

namespace Rapier.Configuration.Settings
{
    public class EntitySettingsContainer : List<IEntitySettings>
    {
        public EntitySettingsContainer(IEnumerable<IEntitySettings> settings)
        {
            AddRange(settings);
        }
    }
}
