using DrumbleApp.Shared.Entities;
using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IPathRepository
    {
        void Insert(DrumbleApp.Shared.Entities.Path path);

        IEnumerable<DrumbleApp.Shared.Entities.Path> GetAllCached(IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators);

        DrumbleApp.Shared.Entities.Path GetPinned(Guid tripId, IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators);

        bool ExistsCached();
    }
}
