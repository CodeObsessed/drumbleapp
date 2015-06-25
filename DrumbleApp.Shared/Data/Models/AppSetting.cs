using DrumbleApp.Shared.Enums;
using GalaSoft.MvvmLight;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class AppSetting : ViewModelBase
    {
        // Define cache setting Id: private field, public property, and database column.
        private int id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL IDENTITY", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    RaisePropertyChanging("Id");
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        // Define application setting: private field, public property, and database column.
        private ApplicationSetting applicationSetting;

        [Column]
        public ApplicationSetting ApplicationSetting
        {
            get { return applicationSetting; }
            set
            {
                if (applicationSetting != value)
                {
                    RaisePropertyChanging("ApplicationSetting");
                    applicationSetting = value;
                    RaisePropertyChanged("ApplicationSetting");
                }
            }
        }

        // Define value: private field, public property, and database column.
        private bool settingValue;

        [Column]
        public bool SettingValue
        {
            get { return settingValue; }
            set
            {
                if (this.settingValue != value)
                {
                    RaisePropertyChanging("SettingValue");
                    this.settingValue = value;
                    RaisePropertyChanged("SettingValue");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public AppSetting(ApplicationSetting appSetting, bool value)
        {
            this.ApplicationSetting = appSetting;
            this.SettingValue = value;
        }

        public AppSetting()
        {
        }
    }
}
