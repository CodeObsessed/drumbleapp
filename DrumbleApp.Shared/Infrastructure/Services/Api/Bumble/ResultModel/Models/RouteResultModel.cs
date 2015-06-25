using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public class RouteResultModel
    {
        public string Name { get; set; }
        public string VehicleNumber { get; set; }
        public string Colour { get; set; }
        public AnnouncementResultModel Announcement { get; set; }
        public IEnumerable<RouteStopResultModel> RouteStops { get; set; }
    }
}
