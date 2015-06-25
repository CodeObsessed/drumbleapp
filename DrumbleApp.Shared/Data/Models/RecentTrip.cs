using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public class Recent : ViewModelBase
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
                    this.id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private string text;

        [Column]
        public string Text
        {
            get { return text; }
            set
            {
                if (this.text != value)
                {
                    RaisePropertyChanging("Text");
                    this.text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        private double latitude;

        [Column]
        public double Latitude
        {
            get { return latitude; }
            set
            {
                if (latitude != value)
                {
                    RaisePropertyChanging("Latitude");
                    this.latitude = value;
                    RaisePropertyChanged("Latitude");
                }
            }
        }

        private double longitude;

        [Column]
        public double Longitude
        {
            get { return longitude; }
            set
            {
                if (longitude != value)
                {
                    RaisePropertyChanging("Longitude");
                    this.longitude = value;
                    RaisePropertyChanged("Longitude");
                }
            }
        }

        private DateTime lastUsedDate;

        [Column]
        public DateTime LastUsedDate
        {
            get { return lastUsedDate; }
            set
            {
                if (lastUsedDate != value)
                {
                    RaisePropertyChanging("LastUsedDate");
                    this.lastUsedDate = value;
                    RaisePropertyChanged("LastUsedDate");
                }
            }
        }

        private DateTime createdDate;

        [Column]
        public DateTime CreatedDate
        {
            get { return createdDate; }
            set
            {
                if (createdDate != value)
                {
                    RaisePropertyChanging("CreatedDate");
                    this.createdDate = value;
                    RaisePropertyChanged("CreatedDate");
                }
            }
        }

        private bool isFavourite;

        [Column]
        public bool IsFavourite
        {
            get { return isFavourite; }
            set
            {
                if (isFavourite != value)
                {
                    RaisePropertyChanging("IsFavourite");
                    this.isFavourite = value;
                    RaisePropertyChanged("IsFavourite");
                }
            }
        }

        private int numberOfUses;

        [Column]
        public int NumberOfUses
        {
            get { return numberOfUses; }
            set
            {
                if (numberOfUses != value)
                {
                    RaisePropertyChanging("NumberOfUses");
                    this.numberOfUses = value;
                    RaisePropertyChanged("NumberOfUses");
                }
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

        public Recent()
        { }

        public Recent(Guid id, Coordinate point, string text, DateTime createdDateUtc, DateTime lastUsedDateUtc, int numberOfUses, bool isFavourite) 
        {
            this.Id = id;
            this.Latitude = point.Latitude;
            this.Longitude = point.Longitude;
            this.Text = text;
            this.CreatedDate = createdDateUtc;
            this.LastUsedDate = lastUsedDateUtc;
            this.NumberOfUses = numberOfUses;
            this.IsFavourite = isFavourite;
        }
    }
}
