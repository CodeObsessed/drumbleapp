using System;
using System.Device.Location;
using System.Globalization;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class Coordinate

    {
        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public GeoCoordinate GeoCoordinate { get; private set; }

        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            GeoCoordinate = new GeoCoordinate(latitude, longitude);
        }

        public bool IsValid()
        {
            if (Double.IsNaN(Latitude) || Double.IsNaN(Longitude))
                return false;

            return true;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}, {1}", Latitude.ToString(CultureInfo.InvariantCulture), Longitude.ToString(CultureInfo.InvariantCulture));
        }

        public double DistanceToCoordinateInMetres(Coordinate coordinate)
        {
            if (coordinate == null)
                return 0.0;

            if (this.Latitude == coordinate.Latitude && this.Longitude == coordinate.Longitude)
                return 0.0;

            var theta = this.Longitude - coordinate.Longitude;

            var distance = Math.Sin(deg2rad(this.Latitude)) * Math.Sin(deg2rad(coordinate.Latitude)) +
                           Math.Cos(deg2rad(this.Latitude)) * Math.Cos(deg2rad(coordinate.Latitude)) *
                           Math.Cos(deg2rad(theta));

            distance = Math.Acos(distance);
            if (double.IsNaN(distance))
                return 0.0;

            distance = rad2deg(distance);
            distance = distance * 60.0 * 1.1515 * 1609.344;

            return (distance);
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        /*public override bool Equals(object obj)
        {
            Coordinate other = (Coordinate) obj;

            return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
        }*/

        public static bool operator ==(Coordinate a, Coordinate b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Latitude == b.Latitude && a.Longitude == b.Longitude;
        }

        public static bool operator !=(Coordinate a, Coordinate b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Coordinate FromGeoCoordinate(GeoCoordinate coordinate)
        {
            return new Coordinate(coordinate.Latitude, coordinate.Longitude);
        }
    }
}
