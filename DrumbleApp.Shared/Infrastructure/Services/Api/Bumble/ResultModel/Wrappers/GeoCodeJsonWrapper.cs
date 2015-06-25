using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Wrappers
{
    public class GeoCodeJsonWrapper
    {
        public IEnumerable<LocationResultModel> Locations { get; set; }
    }
}
