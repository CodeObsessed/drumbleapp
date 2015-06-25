using System;

namespace DrumbleApp.Shared.Entities
{
    public class UberTrip
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string Location { get; private set; }
        public string Destination { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public UberTrip(Guid id, Guid requestId, string location, string destination, DateTime createdDate)
            : this (requestId, location, destination, createdDate)
        {
            this.Id = id;
        }

        public UberTrip(Guid requestId, string location, string destination, DateTime createdDate)
        {
            this.Id = Guid.NewGuid();
            this.RequestId = requestId;
            this.Location = location;
            this.Destination = destination;
            this.CreatedDate = createdDate;
        }
    }
}
