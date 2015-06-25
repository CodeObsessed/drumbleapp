using DrumbleApp.Domain.Models.Entities;
using System;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Interfaces
{
    public interface IPublicTransportOperatorRepository
    {
        void Insert(PublicTransportOperator publicTransportOperator);

        void InsertRange(IEnumerable<PublicTransportOperator> publicTransportOperators);

        IEnumerable<PublicTransportOperator> PublicTransportOperators  { get; }

        PublicTransportOperator GetById(Guid operatorId);
    }
}
