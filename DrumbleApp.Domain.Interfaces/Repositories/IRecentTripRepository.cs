using DrumbleApp.Domain.Models.Entities;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Interfaces
{
    public interface IRecentTripRepository
    {
        IEnumerable<Recent> RecentTrips { get; }

        void Insert(Recent recentTrip);

        Recent MostRecentTrip { get; }

        Recent MostFrequentTrip  { get; }

        IEnumerable<Recent> GetByName(string searchText);

        void Update(Recent recentTrip);

        Recent GetById(System.Guid id);
    }
}
