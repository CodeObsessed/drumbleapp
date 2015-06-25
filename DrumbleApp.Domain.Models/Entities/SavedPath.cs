using DrumbleApp.Domain.Models.ValueObjects;
using System;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class SavedPath
    {
        public int Order { get; private set; }
        public Guid TripId { get; private set; }
        public string LocationText { get; private set; }
        public string DestinationText { get; private set; }
        public Coordinate Location { get; private set; }
        public Coordinate Destination { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string JsonObject { get; private set; }
        public bool IsPinned { get; set; }
        //public PathOption PathOption { get; private set; }

        /*public SavedPath(Guid tripId, int order, DateTime startDate, DateTime endDate, string locationText, string destinationText, Coordinate location, Coordinate destination, bool isPinned, PathOption pathOption, string jsonObject)
        {
            this.Order = order;
            this.TripId = tripId;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.LocationText = locationText;
            this.DestinationText = destinationText;
            this.Location = location;
            this.Destination = destination;
            this.IsPinned = isPinned;
            this.PathOption = pathOption;
            this.JsonObject = jsonObject;
        }*/
    }
}
