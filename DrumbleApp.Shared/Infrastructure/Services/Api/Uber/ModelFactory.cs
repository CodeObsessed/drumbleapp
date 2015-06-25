using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.Model;
using DrumbleApp.Shared.Infrastructure.Services.Api.Uber.ResultModel;
using System;

namespace DrumbleApp.Shared.Infrastructure.Services.Api.Uber
{
    internal sealed class ModelFactory
    {
        internal static UberPrice Create(UberPriceResultModel uberPriceResultModel)
        {
            if (uberPriceResultModel == null)
            {
                throw new ArgumentNullException("uberPriceResultModel");
            }

            return new UberPrice(uberPriceResultModel.product_id, uberPriceResultModel.currency_code, uberPriceResultModel.estimate, uberPriceResultModel.low_estimate, uberPriceResultModel.high_estimate, uberPriceResultModel.duration, uberPriceResultModel.distance);
        }

        internal static UberTime Create(UberTimeResultModel uberTimeResultModel)
        {
            if (uberTimeResultModel == null)
            {
                throw new ArgumentNullException("uberTimeResultModel");
            }

            return new UberTime(uberTimeResultModel.product_id, uberTimeResultModel.estimate);
        }
    }
}
