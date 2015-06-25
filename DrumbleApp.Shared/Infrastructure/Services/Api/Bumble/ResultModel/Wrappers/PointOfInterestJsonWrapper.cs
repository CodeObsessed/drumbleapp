using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers
{
    public sealed class PointOfInterestJsonWrapper
    {
        public IEnumerable<LocationResultModel> Locations { get; set; }
    }
}
