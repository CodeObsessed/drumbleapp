using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model
{
    internal sealed class UberTime
    {
        public Guid ProductId { get; private set; }
        public int TimeEstimateInSeconds { get; private set; }

        public UberTime(Guid productId, int estimateInSeconds)
        {
            this.ProductId = productId;
            this.TimeEstimateInSeconds = estimateInSeconds;
        }
    }
}
