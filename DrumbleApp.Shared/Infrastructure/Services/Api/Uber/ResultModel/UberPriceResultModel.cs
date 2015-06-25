using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel
{
    internal sealed class UberPriceResultModel
    {
        public Guid product_id { get; set; }
        public string currency_code { get; set; }
        public string display_name { get; set; }
        public string estimate { get; set; }
        public string low_estimate { get; set; }
        public string high_estimate { get; set; }
        public double surge_multiplier { get; set; }
        public int duration { get; set; }
        public double distance { get; set; }
    }
}
