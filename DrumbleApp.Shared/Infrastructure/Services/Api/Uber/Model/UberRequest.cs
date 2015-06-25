using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model
{
    public sealed class UberRequest
    {
        public string request_id { get; set; }
        public string status { get; set; }
        public int eta { get; set; }
        public double surge_multiplier { get; set; }
        public string surge_multiplier_href { get; set; }
        public string surge_confirmation_id { get; set; }
        public UberDriver driver { get; set; }
        public UberVehicle vehicle { get; set; }
        public UberLocation location { get; set; }

        internal bool IsEmpty()
        {
            return String.IsNullOrEmpty(request_id)
                && String.IsNullOrEmpty(status)
                && eta == 0
                && surge_multiplier == 0.0
                && String.IsNullOrEmpty(surge_multiplier_href)
                && String.IsNullOrEmpty(surge_confirmation_id)
                && driver == null
                && vehicle == null
                && location == null;
        }
    }
}
