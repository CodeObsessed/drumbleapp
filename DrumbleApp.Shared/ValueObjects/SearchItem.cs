using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Messages.Classes;
using GalaSoft.MvvmLight.Command;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class SearchItem
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Distance { get; private set; }
        public Coordinate Location { get; private set; }

        public string DistanceFromUserLocation
        {
            get
            {
                if (Distance == -1)
                    return string.Empty;

                return DistanceConverter.MetersToText((int) Distance);
            }
        }

        public SearchItem(string name, string description, double latitude, double longitude, double distance)
        {
            this.Name = name;
            this.Description = description;
            this.Distance = distance;
            this.Location = new Coordinate(latitude, longitude);
        }

        public RelayCommand MapSelectCommand
        {
            get { return new RelayCommand(this.MapSelect); }
        }

        private void MapSelect()
        {
            SearchItemMessage.Send(this, Messages.Enums.SearchItemMessageReason.ViewOnMap);
        }
    }
}
