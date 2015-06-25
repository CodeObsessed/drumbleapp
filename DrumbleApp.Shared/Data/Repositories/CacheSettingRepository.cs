using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public sealed class CacheSettingRepository : GenericRepository<CacheSetting>, ICacheSettingRepository
    {
        public CacheSettingRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.CacheSetting cacheSetting)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(cacheSetting));
        }

        public IEnumerable<Entities.CacheSetting> GetAllCacheSettings()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public void Update(Entities.CacheSetting cacheSetting)
        {
            CacheSetting cacheSettingToUpdate = base.DbSet.Where(x => x.Id == cacheSetting.Id).FirstOrDefault();
            cacheSettingToUpdate.LastRefreshedDateUtc = cacheSetting.LastRefreshedDateUtc;
        }

        public Entities.CacheSetting GetByType(Enums.ResourceType resourceType)
        {
            return EntityModelFactory.Create(base.DbSet.Where(x => x.ResourceType == resourceType).FirstOrDefault());
        }
    }
}
