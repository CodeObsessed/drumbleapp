using System.Collections.Generic;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class TripRoute
    {
        public string Colour { get; private set; }
        public IEnumerable<Coordinate> Coordinates { get; private set; }

        public TripRoute(string colour, IEnumerable<Coordinate> coordinates)
        {
            this.Colour = colour;
            this.Coordinates = coordinates;
        }
    }
}
