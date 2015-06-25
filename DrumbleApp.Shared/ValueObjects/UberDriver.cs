using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberDriver
    {
        public string PhoneNumber { get; private set; }
        public double Rating { get; private set; }
        public Uri PictureUrl { get; private set; }
        public string Name { get; private set; }

        public UberDriver(string phoneNumber, double rating, Uri pictureUrl, string name)
        {
            this.PhoneNumber = phoneNumber;
            this.Rating = rating;
            this.PictureUrl = pictureUrl;
            this.Name = name;
        }
    }
}
