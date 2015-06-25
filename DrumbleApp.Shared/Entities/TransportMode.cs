using DrumbleApp.Shared.Enums;

namespace DrumbleApp.Shared.Entities
{
    public sealed class TransportMode
    {
        public int Id { get; private set; }
        public ApplicationTransportMode ApplicationTransportMode { get; private set; }
        public bool IsEnabled { get; set; }

        public TransportMode(int id, ApplicationTransportMode applicationTransportMode, bool isEnabled)
            : this(applicationTransportMode, isEnabled)
        {
            this.Id = id;
        }

        public TransportMode(ApplicationTransportMode applicationTransportMode, bool isEnabled)
        {
            this.ApplicationTransportMode = applicationTransportMode;
            this.IsEnabled = isEnabled;
        }

        public string ModeImage
        {
            get
            {
                switch (this.ApplicationTransportMode)
                {
                    case ApplicationTransportMode.Bus:
                        return "/Images/64/W/ModeBus.png";
                    case ApplicationTransportMode.Rail:
                        return "/Images/64/W/ModeRail.png";
                    case ApplicationTransportMode.Taxi:
                        return "/Images/64/W/ModeTaxi.png";
                    case ApplicationTransportMode.Boat:
                        return "/Images/64/W/ModeBoat.png";
                    case ApplicationTransportMode.Pedestrian:
                        return "/Images/64/W/ModePedestrian.png";
                    default:
                        return "/Images/64/W/ModePedestrian.png";
                }
            }
        }

        public static TransportMode FromString(string mode)
        {
            switch (mode)
            {
                case "Bus":
                    return new TransportMode(ApplicationTransportMode.Bus, true);
                case "Rail":
                    return new TransportMode(ApplicationTransportMode.Rail, true);
                case "Taxi":
                    return new TransportMode(ApplicationTransportMode.Taxi, true);
                case "Boat":
                    return new TransportMode(ApplicationTransportMode.Boat, true);
                case "Pedestrian":
                    return new TransportMode(ApplicationTransportMode.Pedestrian, true);
                default:
                    return new TransportMode(ApplicationTransportMode.Pedestrian, true);
            }
        }
    }
}
