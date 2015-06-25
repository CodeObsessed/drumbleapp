using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IRecentTripRepository
    {
        IEnumerable<Entities.Recent> GetAll();

        void Insert(Entities.Recent recentTrip);

        Entities.Recent GetMostRecent();

        Entities.Recent GetMostFrequent();

        IEnumerable<Entities.Recent> GetByName(string searchText);

        void Update(Entities.Recent recentTrip);

        Entities.Recent GetById(System.Guid id);
    }
}
