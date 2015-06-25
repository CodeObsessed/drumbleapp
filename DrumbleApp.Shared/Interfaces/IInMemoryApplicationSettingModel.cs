using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IInMemoryApplicationSettingModel
    {
        AppSetting GetSetting(ApplicationSetting applicationSetting);

        void UpdateSetting(ApplicationSetting applicationSetting, bool value);
    }
}
