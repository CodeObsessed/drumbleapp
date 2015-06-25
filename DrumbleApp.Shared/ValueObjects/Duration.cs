using System;

namespace DrumbleApp.Shared.ValueObjects
{
    public class Duration
    {
        public int Days { get; private set; }
        public int Hours { get; private set; }
        public int Minutes { get; private set; }
        public double TotalMinutes { get; private set; }

        public Duration(int days, int hours, int minutes)
        {
            this.Days = days;
            this.Hours = hours;
            this.Minutes = minutes;

            this.TotalMinutes = new TimeSpan(days, hours, minutes, 0).TotalMinutes;
        }

        public static double operator +(Duration d1, Duration d2)
        {
            return d1.TotalMinutes + d2.TotalMinutes;
        }
    }
}
