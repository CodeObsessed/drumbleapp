using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class StagePointResultModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Time { get; set; }
        public string Type { get; set; }
        public string PlatformNumber { get; set; }
        public PointResultModel Point { get; set; }
        public IEnumerable<AnnouncementResultModel> Announcements { get; set; }
    }
}
