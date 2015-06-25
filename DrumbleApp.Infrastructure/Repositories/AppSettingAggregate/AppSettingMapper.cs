using DrumbleApp.Domain.Models.Entities;
using DrumbleApp.Infrastructure.Models;
using System;

namespace DrumbleApp.Infrastructure.Repositories.AppSettingAggregate
{
    internal static class AppSettingMapper
    {
        public static AppSettingTable Create(AppSetting appSetting)
        {
            if (appSetting == null)
                throw new ArgumentNullException("appSetting");

            return new AppSettingTable(appSetting.ApplicationSetting, appSetting.Value);
        }

        public static AppSetting Create(AppSettingTable appSettingTable)
        {
            if (appSettingTable == null)
                throw new ArgumentNullException("appSettingTable");

            return new AppSetting(appSettingTable.Id, appSettingTable.ApplicationSetting, appSettingTable.SettingValue);
        }
    }
}
