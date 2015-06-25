using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class StopResultModel
    {
        public string Name { get; set; }
        public string Operator { get; set; }
        public string Mode { get; set; }
        public IEnumerable<StopPointResultModel> StopLocations { get; set; }
    }
}
