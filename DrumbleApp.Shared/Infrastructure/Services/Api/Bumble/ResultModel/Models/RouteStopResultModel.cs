using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public class RouteStopResultModel
    {
        public StopPointResultModel Stop { get; set; }
        public DateTime Time { get; set; }
    }
}
