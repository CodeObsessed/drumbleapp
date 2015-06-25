using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;
using System.Linq;

namespace DrumbleApp.Shared.Entities
{
    public class Announcement
    {
        public string Description { get; private set; }
        public string Type { get; private set; }
        public string OperatorName { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public Coordinate Location { get; private set; }
        public string RelativeDateString { get; private set; }

        private ObservableCollection<string> modeImages = new ObservableCollection<string>();
        public ObservableCollection<string> ModeImages
        {
            get
            {
                return modeImages;
            }
        }

        private ObservableCollection<string> modes = new ObservableCollection<string>();
        public ObservableCollection<string> Modes
        {
            get
            {
                return modes;
            }
        }

        public Announcement() { }

        public Announcement(string description, string type, string operatorName, DateTime? startDate, DateTime? endDate, Coordinate location, IEnumerable<string> modes)
        {
            this.OperatorName = operatorName;
            this.Description = description;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Location = location;
            this.Type = type;
            this.Modes.AddRange(modes);
            this.RelativeDateString = TimeConverter.ToRelativeDateString(StartDate, true);

            if (modes != null)
            {
                if (modes.Select(x => x.ToLower()).Contains("bus"))
                    this.ModeImages.Add("/Images/64/W/ModeBus.png");
                if (modes.Select(x => x.ToLower()).Contains("rail"))
                    this.ModeImages.Add("/Images/64/W/ModeRail.png");
                if (modes.Select(x => x.ToLower()).Contains("taxi"))
                    this.ModeImages.Add("/Images/64/W/ModeTaxi.png");
                if (modes.Select(x => x.ToLower()).Contains("boat"))
                    this.ModeImages.Add("/Images/64/W/ModeBoat.png");

                if (!this.ModeImages.Any())
                    this.ModeImages.Add("/Images/64/W/ModeBus.png");
            }
            else
            {
                this.Modes.Add("bus");
                this.ModeImages.Add("/Images/64/W/ModeBus.png");
            }
        }
    }
}
