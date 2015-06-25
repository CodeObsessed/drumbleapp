using DrumbleApp.Domain.Models.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DrumbleApp.Domain.Models.Entities
{
    public class Route
    {
        private static string walkingRouteColour = "#00AEff";

        public string Name { get; set; }
        public string VehicleNumber { get; set; }
        public string Colour { get; set; }
        public Announcement Announcement { get; set; }
        public IEnumerable<RouteStop> RouteStops { get; set; }

        public int IntermediateStopCount
        {
            get
            {
                return RouteStops.Count();
            }
        }

        public Visibility RouteVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && Colour == walkingRouteColour && RouteStops.Count() == 0)
                    return Visibility.Collapsed; // Is walking
                
                return Visibility.Visible;
            }
        }

        public Visibility RouteIntermediateStopHighCountVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && Colour == walkingRouteColour && RouteStops.Count() == 0)
                    return Visibility.Collapsed; // Is walking

                if (RouteStops.Count() >= 10)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility RouteIntermediateStopLowCountVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && Colour == walkingRouteColour && RouteStops.Count() == 0)
                    return Visibility.Collapsed; // Is walking
                
                if (RouteStops.Count() < 10)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility WalkingVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && Colour == walkingRouteColour && RouteStops.Count() == 0)
                    return Visibility.Visible; // Is walking

                return Visibility.Collapsed;
            }
        }

        public static Route Empty
        {
            get
            {
                return new Route() { Name = string.Empty, Colour = walkingRouteColour, RouteStops = new List<RouteStop>() };
            }
        }
    }
}
