using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public class Weather : ViewModelBase
    {
        private Guid id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
        public Guid Id
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

        private string icon;
        [Column]
        public string Icon
        {
            get { return icon; }
            set
            {
                if (this.icon != value)
                {
                    RaisePropertyChanging("Icon");
                    this.icon = value;
                    RaisePropertyChanged("Icon");
                }
            }
        }

        private DateTime? lastRefreshedDate;
        [Column]
        public DateTime? LastRefreshedDate
        {
            get { return lastRefreshedDate; }
            set
            {
                if (lastRefreshedDate != value)
                {
                    RaisePropertyChanging("LastRefreshedDate");
                    lastRefreshedDate = value;
                    RaisePropertyChanged("LastRefreshedDate");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public Weather(Guid id, string iconCode, DateTime? lastRefreshedDate)
        {
            this.Id = id;
            this.icon = iconCode;
            this.LastRefreshedDate = lastRefreshedDate;
        }

        public Weather()
        {
        }
    }
}
