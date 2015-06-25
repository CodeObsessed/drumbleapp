using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public class PathOptionModel
    {
        public Guid TripId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string EstimatedTotalCost { get; set; }
        public int TotalWalkingDistance { get; set; }
        public int InitialWalkingDistance { get; set; }
        public IEnumerable<StageResultModel> Stages { get; set; }
        public IEnumerable<string> FareMessages { get; set; }
    }
}
