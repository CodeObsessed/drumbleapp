using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models
{
    public sealed class MapRouteResultModel
    {
        public string Colour { get; set; }
        public IEnumerable<PointResultModel> Points { get; set; }
    }
}
