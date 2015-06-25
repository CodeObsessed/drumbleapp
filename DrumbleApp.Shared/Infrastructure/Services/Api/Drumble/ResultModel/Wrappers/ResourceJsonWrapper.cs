using DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Drumble.ResultModel.Wrappers
{
    public class ResourceJsonWrapper
    {
        public IEnumerable<ResourceResultModel> Resources { get; set; }
    }
}
