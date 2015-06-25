using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Models
{
    public sealed class InMemoryApplicationSettingModel : IInMemoryApplicationSettingModel
    {
        private IUnitOfWork unitOfWork;
        private IEnumerable<AppSetting> appSettings;

        public InMemoryApplicationSettingModel(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.appSettings = unitOfWork.AppSettingRepository.GetAll();
        }

        public AppSetting GetSetting(ApplicationSetting applicationSetting)
        {
            return appSettings.Where(x => x.ApplicationSetting == applicationSetting).FirstOrDefault();
        }

        public void UpdateSetting(ApplicationSetting applicationSetting, bool value)
        {
            this.appSettings.Where(x => x.ApplicationSetting == applicationSetting).FirstOrDefault().UpdateSetting(this.unitOfWork, value);
        }
    }
}
