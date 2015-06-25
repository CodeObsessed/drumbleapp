using DrumbleApp.Shared.Enums;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IWeatherRepository
    {
        void Insert(DrumbleApp.Shared.Entities.Weather weather);

        DrumbleApp.Shared.Entities.Weather Get();

        void DeleteAll();
    }
}
