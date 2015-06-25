using DrumbleApp.Shared.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public class About : ViewModelBase
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

        private AboutItem aboutItem;
        [Column]
        public AboutItem AboutItem
        {
            get { return aboutItem; }
            set
            {
                if (aboutItem != value)
                {
                    RaisePropertyChanging("AboutItem");
                    aboutItem = value;
                    RaisePropertyChanged("AboutItem");
                }
            }
        }

        private string aboutValue;
        [Column]
        public string AboutValue
        {
            get { return aboutValue; }
            set
            {
                if (this.aboutValue != value)
                {
                    RaisePropertyChanging("AboutValue");
                    this.aboutValue = value;
                    RaisePropertyChanged("AboutValue");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public About(Guid id, AboutItem aboutItem, string value)
        {
            this.Id = id;
            this.AboutItem = aboutItem;
            this.AboutValue = value;
        }

        public About()
        {
        }
    }
}
