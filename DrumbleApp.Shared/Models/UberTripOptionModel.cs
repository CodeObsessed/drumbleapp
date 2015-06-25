using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Models
{
    public sealed class UberTripOptionModel
    {
        public Address Location { get; private set; }
        public Address Destination { get; private set; }
        public UberOption UberOption { get; private set; }
        public string SurgeConfirmationId { get; set; }

        public UberTripOptionModel(Address location, Address destination, UberOption uberOption)
        {
            this.Location = location;
            this.Destination = destination;
            this.UberOption = uberOption;
        }
    }
}
