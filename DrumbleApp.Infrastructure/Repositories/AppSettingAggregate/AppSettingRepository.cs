using DrumbleApp.Domain.Interfaces;
using DrumbleApp.Domain.Models.Entities;
using DrumbleApp.Domain.Models.Enums;
using DrumbleApp.Infrastructure.Base;
using DrumbleApp.Infrastructure.DataEntities;
using DrumbleApp.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Infrastructure.Repositories.AppSettingAggregate
{
    public class AppSettingRepository : GenericRepository<AppSettingTable>, IAppSettingRepository
    {
        public AppSettingRepository(CacheContext context)
            : base(context)
        {
        }

        public IEnumerable<AppSetting> AppSettings
        {
            get
            {
                return base.DbSet.Select(x => AppSettingMapper.Create(x));
            }
        }

        public void Insert(AppSetting appSetting)
        {
            base.DbSet.InsertOnSubmit(AppSettingMapper.Create(appSetting));
        }
       
        public AppSetting FindByType(ApplicationSetting applicationSetting)
        {
            return base.DbSet.Where(x => x.ApplicationSetting == applicationSetting).Select(x => AppSettingMapper.Create(x)).Single();
        }

        public void Update(AppSetting appSetting)
        {
            AppSettingTable databaseAppSetting = base.DbSet.Single(x => x.ApplicationSetting == appSetting.ApplicationSetting);
            databaseAppSetting.SettingValue = appSetting.Value;
        }
    }
}
