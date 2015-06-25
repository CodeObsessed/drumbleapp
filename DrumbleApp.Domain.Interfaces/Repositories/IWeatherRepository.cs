using DrumbleApp.Domain.Models.Entities;

namespace DrumbleApp.Domain.Interfaces
{
    public interface IWeatherRepository
    {
        void Insert(Weather weather);

        Weather Weather { get; }

        void DeleteAll();
    }
}
