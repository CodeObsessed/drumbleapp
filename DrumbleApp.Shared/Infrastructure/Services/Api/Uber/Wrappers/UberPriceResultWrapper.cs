using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Wrappers
{
    internal sealed class UberPriceResultWrapper
    {
        public IEnumerable<UberPriceResultModel> Prices { get; set; }
    }
}
