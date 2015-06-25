using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using System;

namespace DrumbleApp.Shared.Entities
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

        public RelayCommand SelectPointOnMapCommand
        {
            get { return new RelayCommand(SelectPointOnMap); }
        }

        private void SelectPointOnMap()
        {
            PointOnMapMessage.Send(this);
        }
    }
}
