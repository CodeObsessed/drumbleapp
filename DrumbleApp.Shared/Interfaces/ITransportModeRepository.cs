using DrumbleApp.Shared.Enums;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface ITransportModeRepository
    {
        void Insert(Entities.TransportMode transportMode);

        IEnumerable<Entities.TransportMode> GetAll();

        Entities.TransportMode FindByType(ApplicationTransportMode applicationTransportMode);

        void Update(Entities.TransportMode transportMode);
    }
}
