using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface ICacheSettingRepository
    {
        void Insert(DrumbleApp.Shared.Entities.CacheSetting cacheSetting);

        IEnumerable<Entities.CacheSetting> GetAllCacheSettings();

        void Update(Entities.CacheSetting cacheSetting);

        Entities.CacheSetting GetByType(Enums.ResourceType resourceType);
    }
}
