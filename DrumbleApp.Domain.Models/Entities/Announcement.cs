using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DrumbleApp.Domain.Models.ValueObjects;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class Announcement
    {
        public string Description { get; private set; }
        public string Kind { get; private set; }
        public string OperatorName { get; private set; }
        public Time StartDate { get; private set; }
        public Coordinate Location { get; private set; }

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

        public Announcement(string description, string type, string operatorName, DateTime? startDate, Coordinate location, IEnumerable<string> modes)
        {
            if (modes == null)
                throw new ArgumentNullException("modes");

            this.OperatorName = operatorName;
            this.Description = description;
            this.StartDate = new Time(startDate.Value);
            this.Location = location;
            this.Kind = type;
            foreach (string mode in modes)
                this.Modes.Add(mode);

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
