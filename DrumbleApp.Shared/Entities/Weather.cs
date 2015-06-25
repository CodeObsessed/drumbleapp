using System;

namespace DrumbleApp.Shared.Entities
{
    public sealed class Weather
    {
        public Guid Id { get; set; }
        public string Icon { get; set; }
        public string IconCode { get; set; }
        public DateTime? LastRefreshedDate { get; set; }

        public Weather(Guid id, string iconCode, DateTime? lastRefreshedDate)
            : this(iconCode, lastRefreshedDate)
        {
            this.Id = id;
        }

        public Weather(string iconCode, DateTime? lastRefreshedDate)
        {
            this.Id = Guid.NewGuid();
            this.LastRefreshedDate = lastRefreshedDate;
            this.IconCode = iconCode;
            switch (iconCode)
            {
                case "01d":
                    this.Icon = "/Images/Weather/01d.png";
                    break;
                case "01n":
                    this.Icon = "/Images/Weather/01n.png";
                    break;
                case "02d":
                    this.Icon = "/Images/Weather/02d.png";
                    break;
                case "02n":
                    this.Icon = "/Images/Weather/02n.png";
                    break;
                case "03d":
                    this.Icon = "/Images/Weather/03d.png";
                    break;
                case "03n":
                    this.Icon = "/Images/Weather/03n.png";
                    break;
                case "04d":
                    this.Icon = "/Images/Weather/04d.png";
                    break;
                case "04n":
                    this.Icon = "/Images/Weather/04n.png";
                    break;
                case "09d":
                    this.Icon = "/Images/Weather/09d.png";
                    break;
                case "09n":
                    this.Icon = "/Images/Weather/09n.png";
                    break;
                case "10d":
                    this.Icon = "/Images/Weather/10d.png";
                    break;
                case "10n":
                    this.Icon = "/Images/Weather/10n.png";
                    break;
                case "11d":
                    this.Icon = "/Images/Weather/11d.png";
                    break;
                case "11n":
                    this.Icon = "/Images/Weather/11n.png";
                    break;
                case "13d":
                    this.Icon = "/Images/Weather/13d.png";
                    break;
                case "13n":
                    this.Icon = "/Images/Weather/13n.png";
                    break;
                case "50d":
                    this.Icon = "/Images/Weather/50d.png";
                    break;
                case "50n":
                    this.Icon = "/Images/Weather/50n.png";
                    break;
            }
        }
    }
}
