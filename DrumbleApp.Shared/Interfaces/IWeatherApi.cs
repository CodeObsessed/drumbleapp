using DrumbleApp.Shared.Infrastructure.Services.Api.Common;
using DrumbleApp.Shared.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IWeatherApi
    {
        Task<DrumbleApp.Shared.Entities.Weather> GetDataWeather(Coordinate location);
    }
}
