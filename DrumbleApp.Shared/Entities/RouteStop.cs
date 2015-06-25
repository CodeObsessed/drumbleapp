using System;
using System.Windows;

namespace DrumbleApp.Shared.Entities
{
    public class RouteStop
    {
        public PublicStopPoint StopPoint { get; private set; }
        public DateTime Time { get; private set; }
        public string RouteColour { get; private set; }
        public string RouteName { get; private set; }
        public Visibility ArrivalVisibility { get; private set; }
        public Visibility DepartureVisibility { get; private set; }
        public Visibility IntermediateVisibility { get; private set; }

        public string TimeDisplay
        {
            get
            {
                return Time.ToString("HH:mm");
            }
        }

        public RouteStop(PublicStopPoint stopPoint, DateTime time, string routeColour, string routeName, Visibility arrivalVisibility, Visibility departureVisibility, Visibility intermediateVisibility)
        {
            this.StopPoint = stopPoint;
            this.Time = time.ToLocalTime();
            this.RouteColour = routeColour;
            this.RouteName = routeName;
            this.ArrivalVisibility = arrivalVisibility;
            this.DepartureVisibility = departureVisibility;
            this.IntermediateVisibility = intermediateVisibility;
        }
    }
}
