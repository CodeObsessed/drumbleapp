using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using System;

namespace DrumbleApp.Shared.Entities
{
    public sealed class PlaceOfInterest
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public PlaceOfInterestCategory PlaceOfInterestCategory { get; private set; }
        public int DistanceFromUserLocationInMeters { get; private set; }
        public Coordinate Location { get; private set; }

        public string DistanceFromUserLocation
        {
            get
            {
                if (DistanceFromUserLocationInMeters == -1 || DistanceFromUserLocationInMeters == 0)
                    return String.Empty;

                return DistanceConverter.MetersToText(DistanceFromUserLocationInMeters);
            }
        }

        public PlaceOfInterest(string name, PlaceOfInterestCategory placeOfInterestCategory, string address, Coordinate location, int distanceFromUserLocationInMeters)
        {
            this.Name = name;
            this.Address = address;
            this.Location = location;
            this.DistanceFromUserLocationInMeters = distanceFromUserLocationInMeters;
            this.PlaceOfInterestCategory = placeOfInterestCategory;
        }

        public RelayCommand SelectPointOnMapCommand
        {
            get { return new RelayCommand(SelectPointOnMap); }
        }

        private void SelectPointOnMap()
        {
            PointOnMapMessage.Send(this);
        }
    }
}
