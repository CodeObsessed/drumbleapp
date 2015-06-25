using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Wrappers
{
    internal sealed class UberTimeResultWrapper
    {
        public IEnumerable<UberTimeResultModel> Times { get; set; }
    }
}
