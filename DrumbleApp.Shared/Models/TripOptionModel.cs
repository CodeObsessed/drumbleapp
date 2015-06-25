using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.ValueObjects;

namespace DrumbleApp.Shared.Models
{
    public sealed class TripOptionModel
    {
        public Address Address { get; private set; }

        public SearchCategory SearchCategory { get; set; }

        public TripOptionModel()
        {
            this.SearchCategory = SearchCategory.Unspecified;
        }

        public void SetAsAddress(Address address)
        {
            SearchCategory = Enums.SearchCategory.Address;
            this.Address = address;
        }

        public bool IsValid()
        {
            return (SearchCategory != Enums.SearchCategory.Unspecified);
        }

        public Coordinate Location
        {
            get
            {
                switch (SearchCategory)
                {
                    case Enums.SearchCategory.Address:
                        return this.Address.Location;
                }

                return null;
            }
        }

        public string Text
        {
            get
            {
                switch (SearchCategory)
                {
                    case Enums.SearchCategory.Address:
                        return this.Address.ShortAddressText;
                }

                return null;
            }
        }
    }
}
