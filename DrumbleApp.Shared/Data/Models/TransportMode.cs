using DrumbleApp.Shared.Enums;
using GalaSoft.MvvmLight;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class TransportMode : ViewModelBase
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

        private ApplicationTransportMode applicationTransportMode;
        [Column]
        public ApplicationTransportMode ApplicationTransportMode
        {
            get { return applicationTransportMode; }
            set
            {
                if (applicationTransportMode != value)
                {
                    RaisePropertyChanging("ApplicationTransportMode");
                    applicationTransportMode = value;
                    RaisePropertyChanged("ApplicationTransportMode");
                }
            }
        }

        private bool isEnabled;
        [Column]
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (this.isEnabled != value)
                {
                    RaisePropertyChanging("IsEnabled");
                    this.isEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public TransportMode(ApplicationTransportMode applicationTransportMode, bool isEnabled)
        {
            this.ApplicationTransportMode = applicationTransportMode;
            this.IsEnabled = isEnabled;
        }

        public TransportMode()
        {
        }
    }
}
