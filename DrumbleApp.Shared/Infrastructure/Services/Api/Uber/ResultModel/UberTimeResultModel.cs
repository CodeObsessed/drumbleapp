using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel
{
    internal sealed class UberTimeResultModel
    {
        public Guid product_id { get; set; }
        public int estimate { get; set; }
        public string display_name { get; set; }
    }
}
