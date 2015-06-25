using GalaSoft.MvvmLight;
using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Index(Columns = "Name", Name = "indexName")]
    [Index(Columns = "DistanceFromUserLocationInMeters", Name = "indexDistanceFromUserLocationInMeters")]
    [Table]
    public sealed class PublicStop : ViewModelBase
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

        private string operatorName;
        [Column]
        public string OperatorName
        {
            get { return operatorName; }
            set
            {
                if (operatorName != value)
                {
                    RaisePropertyChanging("OperatorName");
                    operatorName = value;
                    RaisePropertyChanged("OperatorName");
                }
            }
        }

        private string mode;
        [Column]
        public string Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    RaisePropertyChanging("Mode");
                    mode = value;
                    RaisePropertyChanged("Mode");
                }
            }
        }

        /*[Column]
        private Guid operatorId;

        private EntityRef<PublicTransportOperator> publicTransportOperator;

        [Association(Storage = "publicTransportOperator", ThisKey = "operatorId", OtherKey = "Id", IsForeignKey = true)]
        public PublicTransportOperator PublicTransportOperator
        {
            get { return publicTransportOperator.Entity; }
            set
            {
                if (publicTransportOperator.Entity != value)
                {
                    RaisePropertyChanging("PublicTransportOperator");
                    publicTransportOperator.Entity = value;
                    RaisePropertyChanged("PublicTransportOperator");
                }
            }
        }*/

        private EntitySet<PublicStopPoint> stopPoints = new EntitySet<PublicStopPoint>();

        [Association(Storage = "stopPoints", ThisKey = "Id", OtherKey = "PublicStopId")]
        public EntitySet<PublicStopPoint> StopPoints
        {
            get
            {
                return stopPoints;
            }
            set
            {
                stopPoints.Assign(value);
            }
        }

        private int distanceFromUserLocationInMeters;
        [Column]
        public int DistanceFromUserLocationInMeters
        {
            get { return distanceFromUserLocationInMeters; }
            set
            {
                if (distanceFromUserLocationInMeters != value)
                {
                    RaisePropertyChanging("DistanceFromUserLocationInMeters");
                    distanceFromUserLocationInMeters = value;
                    RaisePropertyChanged("DistanceFromUserLocationInMeters");
                }
            }
        }

        [Column(IsVersion = true)]
        private Binary version;

        public PublicStop()
        {
        }

        public PublicStop(Guid id, string name, string operatorName, string mode, IEnumerable<PublicStopPoint> stopPoints)
        {
            this.Id = id;
            this.Name = name;
            this.OperatorName = operatorName;
            this.Mode = mode;
            this.StopPoints.AddRange(stopPoints);
        }
    }
}
