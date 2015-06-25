using DrumbleApp.Shared.Data.Factories;
using DrumbleApp.Shared.Data.Models;
using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Repositories
{
    public class AppSettingRepository : GenericRepository<AppSetting>, IAppSettingRepository
    {
        public AppSettingRepository(CacheContext context)
            : base(context)
        {

        }

        public void Insert(DrumbleApp.Shared.Entities.AppSetting appSetting)
        {
            base.DbSet.InsertOnSubmit(DataModelFactory.Create(appSetting));
        }

        public IEnumerable<DrumbleApp.Shared.Entities.AppSetting> GetAll()
        {
            return base.DbSet.Select(x => EntityModelFactory.Create(x));
        }

        public DrumbleApp.Shared.Entities.AppSetting FindByType(ApplicationSetting applicationSetting)
        {
            return base.DbSet.Where(x => x.ApplicationSetting == applicationSetting).Select(x => EntityModelFactory.Create(x)).Single();
        }

        public void Update(DrumbleApp.Shared.Entities.AppSetting appSetting)
        {
            AppSetting databaseAppSetting = base.DbSet.Single(x => x.ApplicationSetting == appSetting.ApplicationSetting);
            databaseAppSetting.SettingValue = appSetting.Value;
        }
    }
}
