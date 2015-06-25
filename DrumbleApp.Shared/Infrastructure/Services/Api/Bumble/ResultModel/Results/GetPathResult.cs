using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Results
{
    public class GetPathResult
    {
        public IEnumerable<PathOptionModel> Results { get; set; }
    }
}
