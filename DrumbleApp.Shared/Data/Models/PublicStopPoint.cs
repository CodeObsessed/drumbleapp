using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public class PublicStopPoint : ViewModelBase
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

        private string name;

        [Column]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    RaisePropertyChanging("Name");
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string address;

        [Column]
        public string Address
        {
            get { return address; }
            set
            {
                if (address != value)
                {
                    RaisePropertyChanging("Address");
                    address = value;
                    RaisePropertyChanged("Address");
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
                    latitude = value;
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
                    longitude = value;
                    RaisePropertyChanged("Longitude");
                }
            }
        }

        private Guid publicStopId;

        [Column]
        public Guid PublicStopId
        {
            get { return publicStopId; }
            set
            {
                if (publicStopId != value)
                {
                    RaisePropertyChanging("PublicStopId");
                    publicStopId = value;
                    RaisePropertyChanged("PublicStopId");
                }
            }
        }

        public Coordinate Location
        {
            get
            {
                if (this.Latitude == -1 && this.Longitude == -1)
                    return null;

                return new Coordinate(this.Latitude, this.Longitude);
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

        public PublicStopPoint()
        {

        }

        public PublicStopPoint(Guid id, string name, Guid publicStopId, string address, Coordinate location)
        {
            this.Id = id;
            this.Name = name;
            this.Address = address;
            this.PublicStopId = publicStopId;
            this.latitude = (location == null) ? -1 : location.Latitude;
            this.longitude = (location == null) ? -1 : location.Longitude;
        }
    }
}
