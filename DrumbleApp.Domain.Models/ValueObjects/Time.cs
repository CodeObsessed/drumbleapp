using DrumbleApp.Domain.Models.Resources;
using System;
using System.Globalization;
using System.Text;

namespace DrumbleApp.Domain.Models.ValueObjects
{
    public sealed class Time
    {
        private TimeSpan timeSpan;

        public DateTime Value { get; private set; }

        public string Text 
        { 
            get
            {
                return this.ToString();    
            }  
        }

        public string RelativeText
        {
            get
            {
                return this.ToRelativeString();
            }
        }

        public Time(DateTime value)
        {
            this.timeSpan = new TimeSpan(value.Ticks);
            this.Value = value;
        }

        public Time(TimeSpan value)
        {
            this.timeSpan = value;
            this.Value = new DateTime(value.Ticks);
        }

        public override string ToString()
        {
            if (this.timeSpan.TotalSeconds <= 60)
                return "< 1" + AppResources.MinuteAbbr;
            else if (this.timeSpan.TotalSeconds < 3600)
                return this.timeSpan.TotalMinutes.ToString(CultureInfo.InvariantCulture) + AppResources.MinuteAbbr;
            else
            {
                string minuteText = this.timeSpan.Minutes.ToString(CultureInfo.InvariantCulture) + AppResources.MinuteAbbr;
                if (this.timeSpan.Minutes == 0)
                    minuteText = string.Empty;

                return (this.timeSpan.Hours.ToString(CultureInfo.InvariantCulture) + AppResources.HourAbbr + " " + minuteText).TrimEnd();
            }
        }

        /// <summary>
        /// Compares a supplied date to the current date and generates a friendly English 
        /// comparison ("5 days ago", "5 days from now")
        /// </summary>
        /// <param name="approximate">When off, calculate timespan down to the second.
        /// When on, approximate to the largest round unit of time.</param>
        /// <returns></returns>
        public string ToRelativeString(bool approximate = true)
        {
            StringBuilder sb = new StringBuilder();

            string suffix = (this.Value > DateTime.Now) ? " " + AppResources.FromNow : " " + AppResources.Ago;

            TimeSpan timeSpan = new TimeSpan(Math.Abs(DateTime.Now.Subtract(Value).Ticks));

            if (timeSpan.Days > 0)
            {
                sb.AppendFormat("{0} {1}", timeSpan.Days,
                  (timeSpan.Days > 1) ? AppResources.Days : AppResources.Day);
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Hours > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Hours, (timeSpan.Hours > 1) ? AppResources.Hours : AppResources.Hour);
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Minutes > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Minutes, (timeSpan.Minutes > 1) ? AppResources.Minutes : AppResources.Minute);
                if (approximate) return sb.ToString() + suffix;
            }
            if (timeSpan.Seconds > 0)
            {
                sb.AppendFormat("{0}{1} {2}", (sb.Length > 0) ? ", " : string.Empty,
                  timeSpan.Seconds, (timeSpan.Seconds > 1) ? AppResources.Seconds : AppResources.Second);
                if (approximate) return sb.ToString() + suffix;
            }
            if (sb.Length == 0) return AppResources.RightNow;

            sb.Append(suffix);
            return sb.ToString();
        }
    }
}
