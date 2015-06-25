using DrumbleApp.Shared.Converters;
using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class UberRequest
    {
        public Guid RequestId { get; private set; }
        public string Status { get; private set; }
        public double SurgeMultiplier { get; private set; }
        public Uri SurgeMultiplierHref { get; private set; }
        public string SurgeConfirmationId { get; private set; }
        public int Eta { get; private set; }
        public UberVehicle UberVehicle { get; private set; }
        public UberDriver UberDriver { get; private set; }
        public Coordinate Location { get; private set; }

        public UberRequest(Guid requestId, string status, double surgeMultiplier, int eta, UberVehicle uberVehicle, UberDriver uberDriver, Coordinate location, Uri surgeMultiplierHref, string surgeConfirmationId)
        {
            this.RequestId = requestId;
            this.Status = status;
            this.SurgeMultiplier = surgeMultiplier;
            this.Eta = eta;
            this.UberDriver = uberDriver;
            this.UberVehicle = uberVehicle;
            this.Location = location;
            this.SurgeMultiplierHref = surgeMultiplierHref;
            this.SurgeConfirmationId = surgeConfirmationId;
        }

        public UberRequest(Guid requestId)
        {
            this.RequestId = requestId;
        }

        public string EtaText
        {
            get
            {
                return TimeConverter.MinutesToText(Eta);
            }
        }
    }
}
