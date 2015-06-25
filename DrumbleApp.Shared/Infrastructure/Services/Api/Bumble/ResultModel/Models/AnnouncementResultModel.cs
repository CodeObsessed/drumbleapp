using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public class AnnouncementResultModel
    {
        public string AnnouncementType { get; set; }
        public string Operator { get; set; }
        public IEnumerable<string> Modes { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PointResultModel Point { get; set; }
    }
}
