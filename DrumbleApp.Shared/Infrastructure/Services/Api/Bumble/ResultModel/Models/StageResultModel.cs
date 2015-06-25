using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public class StageResultModel
    {
        public string Name { get; set; }
        public string Mode { get; set; }
        public string Operator { get; set; }
        public string VehicleNumber { get; set; }
        public double Duration { get; set; }
        public string Cost { get; set; }
        public string Colour { get; set; }
        public string Description { get; set; }
        public IEnumerable<AnnouncementResultModel> Announcements { get; set; }
        public IEnumerable<StagePointResultModel> StageLocations { get; set; }

    }
}
