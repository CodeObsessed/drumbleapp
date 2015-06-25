using DrumbleApp.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class Trip
    {
        public Guid Id { get; set; }
        public Coordinate BoundingBoxTopLeft { get; private set; }
        public Coordinate BoundingBoxBottomRight { get; private set; }
        public IEnumerable<TripRoute> TripRoutes { get; private set; }

        public Trip(Guid id, Coordinate boundingBoxTopLeft, Coordinate boundingBoxBottomRight, IEnumerable<TripRoute> tripRoutes)
        {
            this.Id = id;
            this.BoundingBoxBottomRight = boundingBoxBottomRight;
            this.BoundingBoxTopLeft = boundingBoxTopLeft;
            this.TripRoutes = tripRoutes;
        }
    }
}
