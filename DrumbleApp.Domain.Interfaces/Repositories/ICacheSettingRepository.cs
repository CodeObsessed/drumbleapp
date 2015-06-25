using DrumbleApp.Domain.Models.Entities;
using DrumbleApp.Domain.Models.Enums;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Interfaces
{
    public interface ICacheSettingRepository
    {
        void Insert(CacheSetting cacheSetting);

        IEnumerable<CacheSetting> CacheSettings { get; }

        void Update(CacheSetting cacheSetting);

        CacheSetting GetByType(ResourceType resourceType);
    }
}
