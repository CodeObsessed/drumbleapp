using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Entities
{
    public sealed class PublicStop
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string OperatorName { get; private set; }
        public string Mode { get; private set; }
        public IEnumerable<PublicStopPoint> StopPoints { get; set; }
        
        public Coordinate Location
        {
            get
            {
                return StopPoints.First().Location;
            }
        }

        private int distanceFromUserLocationInMeters;
        public string DistanceFromUserLocation
        {
            get
            {
                if (distanceFromUserLocationInMeters == -1)
                    return String.Empty;

                return DistanceConverter.MetersToText(distanceFromUserLocationInMeters);
            }
        }

        public PublicStop(string name, string operatorName, string mode, int distanceFromUserLocationInMeters)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Mode = mode;
            this.distanceFromUserLocationInMeters = distanceFromUserLocationInMeters;
            this.OperatorName = operatorName;
        }

        public PublicStop(Guid id, string name, string operatorName, string mode, int distanceFromUserLocationInMeters)
            : this(name, operatorName, mode, distanceFromUserLocationInMeters)
        {
            this.Id = id;
        }

        public void SetDistanceFromUserLocation(int distanceInMeters)
        {
            this.distanceFromUserLocationInMeters = distanceInMeters;
        }
    }
}
