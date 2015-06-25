using System.Collections.Generic;

namespace DrumbleApp.Shared.Infrastructure.Services.Weather.ResultModel
{
    public sealed class WeatherResultModel
    {
        public IEnumerable<DrumbleApp.Shared.Infrastructure.Services.Weather.Models.Weather> Weather { get; set; }
    }
}
