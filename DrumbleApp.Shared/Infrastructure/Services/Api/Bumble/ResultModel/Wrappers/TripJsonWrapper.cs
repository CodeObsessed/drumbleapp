using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers
{
    public sealed class TripJsonWrapper
    {
        public PointResultModel BoundingBoxTopLeft { get; set; }
        public PointResultModel BoundingBoxBottomRight { get; set; }
        public IEnumerable<MapRouteResultModel> Routes { get; set; }
    }
}
