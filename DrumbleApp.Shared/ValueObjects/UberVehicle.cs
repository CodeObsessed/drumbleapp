using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberVehicle
    {
        public string Make { get; private set; }
        public string Model { get; private set; }
        public string LicensePlate { get; private set; }
        public Uri PictureUrl { get; private set; }
        public string DisplayText { get; private set; }

        public UberVehicle(string make, string model, string licensePlate, Uri pictureUrl)
        {
            this.Make = make;
            this.Model = model;
            this.LicensePlate = licensePlate;
            this.PictureUrl = pictureUrl;
            this.DisplayText = string.Format("{0} {1}", Make, Model);
        }
    }
}
