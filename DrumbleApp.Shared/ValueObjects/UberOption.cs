using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberOption
    {
        public Guid ProductId { get; private set; }
        public string CurrencyCode { get; private set; }
        public string PriceEstimate { get; private set; }
        public string LowEstimate { get; private set; }
        public string HighEstimate { get; private set; }
        public int DurationInSeconds { get; private set; }
        public double DistanceInMeters { get; private set; }
        public int TimeEstimateInSeconds { get; private set; }
        public string ClientId { get; private set; }

        public UberOption(Guid productId, string currencyCode, string priceEstimate, string lowEstimate, string highEstimate, int durationInSeconds, double distanceInMeters, int timeEstimateInSeconds, string clientId)
        {
            this.ClientId = clientId;
            this.ProductId = productId;
            this.CurrencyCode = currencyCode;
            this.PriceEstimate = priceEstimate;
            this.LowEstimate = lowEstimate;
            this.HighEstimate = highEstimate;
            this.DurationInSeconds = durationInSeconds;
            this.DistanceInMeters = distanceInMeters;
            this.TimeEstimateInSeconds = timeEstimateInSeconds;
        }
    }
}
