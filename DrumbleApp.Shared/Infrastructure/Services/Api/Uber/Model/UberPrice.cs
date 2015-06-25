using DrumbleApp.Shared.Converters;
using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model
{
    internal sealed class UberPrice
    {
        public Guid ProductId { get; private set; }
        public string CurrencyCode { get; private set; }
        public string PriceEstimate { get; private set; }
        public string LowEstimate { get; private set; }
        public string HighEstimate { get; private set; }
        public int DurationInSeconds { get; private set; }
        public double DistanceInMeters { get; private set; }

        public UberPrice(Guid productId, string currencyCode, string priceEstimate, string lowEstimate, string highEstimate, int durationInSeconds, double distanceInMiles)
        {
            this.ProductId = productId;
            this.CurrencyCode = currencyCode;
            this.PriceEstimate = priceEstimate;
            this.LowEstimate = lowEstimate;
            this.HighEstimate = highEstimate;
            this.DurationInSeconds = durationInSeconds;
            this.DistanceInMeters = DistanceConverter.MilesToMeters(distanceInMiles);
        }
    }
}
