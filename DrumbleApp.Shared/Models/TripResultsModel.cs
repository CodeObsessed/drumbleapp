using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Models
{
    public class TripResultsModel
    {
        private List<Trip> trips;
        public List<Trip> Trips 
        {
            get
            {
                return trips;
            }
        }

        public TripResultsModel()
        {
            trips = new List<Trip>();
        }

        public Trip GetTrip(Guid id)
        {
            return Trips.Where(x => x.Id == id).SingleOrDefault();
        }

        public bool Exists(Guid id)
        {
            return Trips.Any(x => x.Id == id);
        }

        public void AddTrip(Trip trip)
        {
            if (!Exists(trip.Id))
            {
                trips.Add(trip);
            }
        }
    }
}
