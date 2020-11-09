using System.Collections.Generic;

namespace Rapier.Configuration
{
    public class EntitySettingsContainer : List<IEntitySetting>
    {
        public EntitySettingsContainer(IEnumerable<IEntitySetting> settings)
        {
            AddRange(settings);
        }
    }
}
