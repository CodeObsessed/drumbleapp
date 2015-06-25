
namespace DrumbleApp.Domain.Models.ValueObjects
{
    public class Address
    {
        public string ShortAddressText { get; private set; }
        public string AddressText { get; private set; }
        public Coordinate Location { get; private set; }

        public Address(string address, string shortAddress, Coordinate location)
        {
            this.AddressText = address;
            this.Location = location;
            this.ShortAddressText = shortAddress;
        }

        public Address(string address, string shortAddress, double latitude, double longitude)
            :this(address, shortAddress, new Coordinate(latitude, longitude))
        {

        }
    }
}
