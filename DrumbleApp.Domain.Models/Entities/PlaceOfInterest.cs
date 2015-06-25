using DrumbleApp.Domain.Models.ValueObjects;
using System;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class PlaceOfInterest
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public Distance DistanceFromUserLocation { get; private set; }
        public Coordinate Location { get; private set; }

        public PlaceOfInterest(string name, string address, Coordinate location, double distanceFromUserLocationInMeters, bool metric)
        {
            this.Name = name;
            this.Address = address;
            this.Location = location;
            this.DistanceFromUserLocation = new Distance(distanceFromUserLocationInMeters, metric);
        }
    }
}
