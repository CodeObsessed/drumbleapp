using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class Path : ViewModelBase
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

        private Guid tripId;
        [Column]
        public Guid TripId
        {
            get { return tripId; }
            set
            {
                if (tripId != value)
                {
                    RaisePropertyChanging("TripId");
                    tripId = value;
                    RaisePropertyChanged("TripId");
                }
            }
        }

        private string locationText;
        [Column]
        public string LocationText
        {
            get { return locationText; }
            set
            {
                if (locationText != value)
                {
                    RaisePropertyChanging("LocationText");
                    locationText = value;
                    RaisePropertyChanged("LocationText");
                }
            }
        }

        private string destinationText;
        [Column]
        public string DestinationText
        {
            get { return destinationText; }
            set
            {
                if (destinationText != value)
                {
                    RaisePropertyChanging("DestinationText");
                    destinationText = value;
                    RaisePropertyChanged("DestinationText");
                }
            }
        }

        private double startLatitude;
        [Column]
        public double StartLatitude
        {
            get { return startLatitude; }
            set
            {
                if (startLatitude != value)
                {
                    RaisePropertyChanging("StartLatitude");
                    startLatitude = value;
                    RaisePropertyChanged("StartLatitude");
                }
            }
        }

        private double startLongitude;
        [Column]
        public double StartLongitude
        {
            get { return startLongitude; }
            set
            {
                if (startLongitude != value)
                {
                    RaisePropertyChanging("StartLongitude");
                    startLongitude = value;
                    RaisePropertyChanged("StartLongitude");
                }
            }
        }

        private double endLatitude;
        [Column]
        public double EndLatitude
        {
            get { return endLatitude; }
            set
            {
                if (endLatitude != value)
                {
                    RaisePropertyChanging("EndLatitude");
                    endLatitude = value;
                    RaisePropertyChanged("EndLatitude");
                }
            }
        }

        private double endLongitude;
        [Column]
        public double EndLongitude
        {
            get { return endLongitude; }
            set
            {
                if (endLongitude != value)
                {
                    RaisePropertyChanging("EndLongitude");
                    endLongitude = value;
                    RaisePropertyChanged("EndLongitude");
                }
            }
        }

        private DateTime startDate;
        [Column]
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (startDate != value)
                {
                    RaisePropertyChanging("StartDate");
                    startDate = value;
                    RaisePropertyChanged("StartDate");
                }
            }
        }

        private DateTime endDate;
        [Column]
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                if (endDate != value)
                {
                    RaisePropertyChanging("EndDate");
                    endDate = value;
                    RaisePropertyChanged("EndDate");
                }
            }
        }

        private bool isPinned;
        [Column]
        public bool IsPinned
        {
            get { return isPinned; }
            set
            {
                if (isPinned != value)
                {
                    RaisePropertyChanging("IsPinned");
                    isPinned = value;
                    RaisePropertyChanged("IsPinned");
                }
            }
        }

        private string jsonObject;
        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string JsonObject
        {
            get { return jsonObject; }
            set
            {
                if (jsonObject != value)
                {
                    RaisePropertyChanging("JsonObject");
                    jsonObject = value;
                    RaisePropertyChanged("JsonObject");
                }
            }
        }

        private int order;
        [Column]
        public int Order
        {
            get { return order; }
            set
            {
                if (order != value)
                {
                    RaisePropertyChanging("Order");
                    order = value;
                    RaisePropertyChanged("Order");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public Path(Guid tripId, string locationText, string destinationText, Coordinate location, Coordinate destination, DateTime startDate, DateTime endDate, bool isPinned, string jsonObject, int order)
        {
            this.Id = Guid.NewGuid();
            this.TripId = tripId;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.LocationText = locationText;
            this.DestinationText = destinationText;
            this.StartLatitude = location.Latitude;
            this.EndLatitude = destination.Latitude;
            this.StartLongitude = location.Longitude;
            this.EndLongitude = destination.Longitude;
            this.IsPinned = isPinned;
            this.JsonObject = jsonObject;
            this.Order = order;
        }

        public Path()
        {
        }
    }
}
