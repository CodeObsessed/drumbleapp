using DrumbleApp.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class PublicStop
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string OperatorName { get; private set; }
        public string Mode { get; private set; }
        public IEnumerable<PublicStopPoint> StopPoints { get; set; }
        
        public Coordinate Location
        {
            get
            {
                return StopPoints.First().Location;
            }
        }

        public PublicStop(string name, string operatorName, string mode)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Mode = mode;
            this.OperatorName = operatorName;
        }

        public PublicStop(Guid id, string name, string operatorName, string mode)
            : this(name, operatorName, mode)
        {
            this.Id = id;
        }
    }
}
