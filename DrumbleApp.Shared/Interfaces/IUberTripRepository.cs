using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IUberTripRepository
    {
        void Insert(DrumbleApp.Shared.Entities.UberTrip uberTrip);

        IEnumerable<DrumbleApp.Shared.Entities.UberTrip> GetAll();

        bool ExistsCached();
    }
}
