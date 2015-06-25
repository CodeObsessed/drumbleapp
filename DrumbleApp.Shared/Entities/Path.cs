using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel;
using DrumbleApp.Shared.Infrastructure.Services.Api.Bumble.ResultModel.Models;
using DrumbleApp.Shared.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DrumbleApp.Shared.Entities
{
    public sealed class Path
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
        public PathOption PathOption { get; private set; }

        public Path(Guid tripId, DateTime startDate, DateTime endDate, string locationText, string destinationText, Coordinate location, Coordinate destination, bool isPinned, string jsonObject, int order, IEnumerable<PublicTransportOperator> publicTransportOperators)
            : this(tripId, startDate, endDate, locationText, destinationText, location, destination, isPinned, jsonObject, order)
        {
            this.PathOption = ModelFactory.Create(JsonConvert.DeserializeObject<PathOptionModel>(jsonObject), publicTransportOperators, order);
        }

        public Path(Guid tripId, DateTime startDate, DateTime endDate, string locationText, string destinationText, Coordinate location, Coordinate destination, bool isPinned, string jsonObject, int order)
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
            this.JsonObject = jsonObject;
        }
    }
}
