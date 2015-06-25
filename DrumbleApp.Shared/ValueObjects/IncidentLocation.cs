
namespace DrumbleApp.Shared.ValueObjects
{
    public class IncidentLocation
    {
        public string RoadWay { get; set; }
        public string Direction { get; set; }
        public string Location { get; set; }

        public IncidentLocation(string roadWay, string direction, string location)
        {
            this.RoadWay = roadWay;
            this.Direction = direction;
            this.Location = location;
        }

        public string Text
        {
            get
            {
                string location = string.Empty;
                if (!string.IsNullOrEmpty(RoadWay))
                    location += RoadWay;
                if (!string.IsNullOrEmpty(Direction))
                    location += (!string.IsNullOrEmpty(location)) ? "," + Direction : Direction;
                if (!string.IsNullOrEmpty(Location))
                    location += (!string.IsNullOrEmpty(location)) ? "," + Location : Location;

                return location;
            }
        }
    }
}
