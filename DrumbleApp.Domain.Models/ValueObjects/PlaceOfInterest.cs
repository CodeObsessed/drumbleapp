
namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class PlaceOfInterest
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }

        public PlaceOfInterest(string name, Address address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
