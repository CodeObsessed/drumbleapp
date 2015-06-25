using DrumbleApp.Domain.Models.Entities;
using GalaSoft.MvvmLight.Command;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class SearchItem
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Distance DistanceFromUserLocation { get; private set; }
        public Coordinate Location { get; private set; }

        public SearchItem(string name, string description, double latitude, double longitude, double distanceInMeters, bool metric)
        {
            this.Name = name;
            this.Description = description;
            this.DistanceFromUserLocation = new Distance(distanceInMeters, metric);
            this.Location = new Coordinate(latitude, longitude);
        }

        /*public RelayCommand MapSelectCommand
        {
            get { return new RelayCommand(this.MapSelect); }
        }

        private void MapSelect()
        {
            SearchItemMessage.Send(this, Messages.Enums.SearchItemMessageReason.ViewOnMap);
        }*/
    }
}
