using DrumbleApp.Domain.Models.ValueObjects;
using System;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class PublicStopPoint
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public Coordinate Location { get; private set; }
        public Guid PublicStopId { get; private set; }

        public PublicStopPoint(string name, string address, Guid publicStopId, Coordinate location)
            : this (name, address, location)
        {
            this.PublicStopId = publicStopId;
        }

        public PublicStopPoint(Guid id, string name, string address, Guid publicStopId, Coordinate location)
            : this(name, address, location)
        {
            this.Id = id;
            this.PublicStopId = publicStopId;
        }

        public PublicStopPoint(string name, string address, Coordinate location)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Address = address;
            this.Location = location;
        }

        public PublicStopPoint(Guid id, string name, string address, Coordinate location)
            : this(name, address, location)
        {
            this.Id = id;
        }
    }
}
