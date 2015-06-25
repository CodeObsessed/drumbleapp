using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;

namespace DrumbleApp.Shared.Entities
{
    public class AppSetting
    {
        public int Id { get; private set; }
        public ApplicationSetting ApplicationSetting { get; private set; }
        public bool Value { get; set; }

        public AppSetting(int id, ApplicationSetting applicationSetting, bool value)
            : this (applicationSetting, value)
        {
            this.Id = id;
        }

        public AppSetting(ApplicationSetting applicationSetting, bool value)
        {
            this.ApplicationSetting = applicationSetting;
            this.Value = value;
        }

        public void UpdateSetting(IUnitOfWork unitOfWork, bool value)
        {
            this.Value = value;

            unitOfWork.AppSettingRepository.Update(this);
            unitOfWork.Save();
        }
    }
}
