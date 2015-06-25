using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public class UberTrip : ViewModelBase
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

        private Guid requestId;
        [Column]
        public Guid RequestId
        {
            get { return requestId; }
            set
            {
                if (this.requestId != value)
                {
                    RaisePropertyChanging("RequestId");
                    this.requestId = value;
                    RaisePropertyChanged("RequestId");
                }
            }
        }

        private string location;
        [Column]
        public string Location
        {
            get { return location; }
            set
            {
                if (this.location != value)
                {
                    RaisePropertyChanging("Location");
                    this.location = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        private string destination;
        [Column]
        public string Destination
        {
            get { return destination; }
            set
            {
                if (this.destination != value)
                {
                    RaisePropertyChanging("Destination");
                    this.destination = value;
                    RaisePropertyChanged("Destination");
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
                if (this.createdDate != value)
                {
                    RaisePropertyChanging("CreatedDate");
                    this.createdDate = value;
                    RaisePropertyChanged("CreatedDate");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public UberTrip(Guid id, Guid requestId, string location, string destination, DateTime createdDate)
        {
            this.Id = id;
            this.RequestId = requestId;
            this.Location = location;
            this.Destination = destination;
            this.CreatedDate = createdDate;
        }

        public UberTrip()
        {
        }
    }
}
