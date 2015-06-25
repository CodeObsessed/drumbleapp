using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IPublicStopRepository
    {
        void Insert(Entities.PublicStop publicStop);

        void InsertRange(IEnumerable<Entities.PublicStop> publicStops);

        IEnumerable<Entities.PublicStop> GetAll();

        IEnumerable<Entities.PublicStop> GetAll(int limit);

        IEnumerable<Entities.PublicStop> GetNearby(int limit);

        void UpdateDistanceToAll(Coordinate userLocation);

        IEnumerable<Entities.PublicStop> GetByName(string searchText);

        Entities.PublicStop FindByName(string name);

        IEnumerable<Entities.PublicStopPoint> GetPointsForMap();

        IEnumerable<Entities.PublicStopPoint> GetNearby(int limit, Coordinate location);

        Entities.PublicStop FindById(Guid publicStopId);
    }
}
