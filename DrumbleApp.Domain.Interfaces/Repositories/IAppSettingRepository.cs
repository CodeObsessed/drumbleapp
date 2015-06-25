using DrumbleApp.Domain.Models.Entities;
using DrumbleApp.Domain.Models.Enums;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Interfaces
{
    public interface IAppSettingRepository
    {
        void Insert(AppSetting appSetting);

        IEnumerable<AppSetting> AppSettings  { get; }

        AppSetting FindByType(ApplicationSetting applicationSetting);

        void Update(AppSetting appSetting);
    }
}
