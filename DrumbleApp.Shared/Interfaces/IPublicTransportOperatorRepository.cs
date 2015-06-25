using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Interfaces
{
    public interface IPublicTransportOperatorRepository
    {
        void Insert(DrumbleApp.Shared.Entities.PublicTransportOperator publicTransportOperator);

        void InsertRange(IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> publicTransportOperators);

        IEnumerable<DrumbleApp.Shared.Entities.PublicTransportOperator> GetAll();

        Entities.PublicTransportOperator GetById(Guid operatorId);
    }
}
