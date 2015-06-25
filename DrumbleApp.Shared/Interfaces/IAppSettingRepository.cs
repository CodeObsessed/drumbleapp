using DrumbleApp.Shared.Enums;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IAppSettingRepository
    {
        void Insert(DrumbleApp.Shared.Entities.AppSetting appUse);

        IEnumerable<DrumbleApp.Shared.Entities.AppSetting> GetAll();

        DrumbleApp.Shared.Entities.AppSetting FindByType(ApplicationSetting applicationSetting);

        void Update(DrumbleApp.Shared.Entities.AppSetting appSetting);
    }
}
