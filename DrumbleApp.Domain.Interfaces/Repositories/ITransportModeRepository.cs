using DrumbleApp.Domain.Models.Entities;
using DrumbleApp.Domain.Models.Enums;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Interfaces
{
    public interface ITransportModeRepository
    {
        void Insert(TransportMode transportMode);

        IEnumerable<TransportMode> TransportModes { get; }

        TransportMode FindByType(ApplicationTransportMode applicationTransportMode);

        void Update(TransportMode transportMode);
    }
}
