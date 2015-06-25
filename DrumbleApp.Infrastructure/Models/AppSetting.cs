using DrumbleApp.Domain.Models.Enums;
using GalaSoft.MvvmLight;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Infrastructure.Models
{
    [Table]
    public sealed class AppSettingTable : ViewModelBase
    {
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [Column(IsVersion = true)]
        private Binary version;

        public AppSettingTable(ApplicationSetting appSetting, bool value)
        {
            this.ApplicationSetting = appSetting;
            this.SettingValue = value;
        }

        public AppSettingTable()
        {
        }
    }
}
