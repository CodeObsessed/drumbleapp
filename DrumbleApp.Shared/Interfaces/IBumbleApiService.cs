using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IBumbleApiService
    {
        Task<ObservableCollection<Country>> Countries();

        Task<IEnumerable<PublicTransportOperator>> Operators(CancellationToken ct, User user);

        Task<Address> ReverseGeoCode(CancellationToken ct, User user, Coordinate location);

        Task<IEnumerable<SearchItem>> Search(CancellationToken ct, User user, string searchText, Coordinate userLocation);

        Task<IEnumerable<PathOption>> Path(CancellationToken ct, User user, Coordinate startLocation, Coordinate endLocation, bool isDeparting, DateTime? date, int? timeOffset, List<string> excludedModes, List<string> excludedOperators);

        Task<IEnumerable<Entities.PlaceOfInterest>> PlacesOfInterest(CancellationToken ct, User user, string searchText, string[] categories);

        Task<Trip> Trip(CancellationToken ct, User user, Guid tripId);

        Task<IEnumerable<Announcement>> Announcements(CancellationToken ct, User user, List<string> excludedModes, List<string> excludedOperators, DateTime? afterDate = null);
    }
}
