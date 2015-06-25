using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model;
using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber
{
    internal sealed class ResultBuilder
    {
        internal static IEnumerable<UberPrice> BuildUberPriceResult(IEnumerable<UberPriceResultModel> uberPrices)
        {
            if (uberPrices == null)
                throw new ArgumentNullException("uberPrices");

            if (!uberPrices.Any())
                return null;

            return uberPrices.Select(x => ModelFactory.Create(x));
        }

        internal static UberTime BuildUberTimeResult(UberTimeResultModel uberTime)
        {
            if (uberTime == null)
                return null;

            return ModelFactory.Create(uberTime);
        }
    }
}
