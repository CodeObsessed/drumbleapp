using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
using DrumbleApp.Domain.Models.ValueObjects;

namespace DrumbleApp.Domain.Models.Entities
{
    public class Stage //: ViewModelBase
    {
        public int Order { get; private set; }
        public string Name { get; private set; }
        public TransportMode Mode { get; private set; }
        public string Operator { get; private set; }
        public double Duration { get; private set; }
        public string Cost { get; private set; }
        public string Colour { get; private set; }
        public string Description { get; private set; }
        public PublicStopPoint StartStopPoint { get; private set; }
        public PublicStopPoint EndStopPoint { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public IEnumerable<Announcement> Announcements { get; private set; }
        public IEnumerable<RouteStop> StagePoints { get; set; }
        public IEnumerable<RouteStop> StagePointDisplays { get; set; }
        public WaitingStage WaitingStage { get; set; }

        public Stage(int order, string name, string mode, string operatorName, double duration, string cost, string colour, string description, IEnumerable<RouteStop> routeStops, IEnumerable<Announcement> announcements)
        {
            this.Order = order;
            this.Name = name;
            this.Mode = TransportMode.FromString(mode);
            this.Operator = operatorName;
            this.Duration = duration;
            this.Cost = cost;
            this.Description = description;
            this.StartStopPoint = routeStops.First().StopPoint;
            this.EndStopPoint = routeStops.Last().StopPoint;
            this.StartTime = routeStops.First().Time;
            this.EndTime = routeStops.Last().Time;
            this.Announcements = announcements;
            this.StagePoints = routeStops;
            this.StagePointDisplays = routeStops.Skip(1).Take(routeStops.Count() - 2);

            if (this.Mode.ApplicationTransportMode == Enums.ApplicationTransportMode.Pedestrian)
            {
                this.Colour = "#FFFFFF";
                //this.ExpandIntermediateStopsDownVisibility = Visibility.Collapsed;
                //this.ExpandIntermediateStopsUpVisibility = Visibility.Collapsed;
            }
            else
            {
                this.Colour = colour;
            }

#if DEBUG
            if (order % 2 == 0)
            {
                List<Announcement> tempAnnouncements = new List<Announcement>();
                //tempAnnouncements.Add(new Announcement("Test routes have been canceled, this is a test, from 18:00 to 19:00, because I have no announcements from the api.", "Delay", "MetroRail", DateTime.Now, DateTime.Now.AddHours(1), null, new List<string>() { "Bus" }));
                //tempAnnouncements.Add(new Announcement("Test routes have been delayed, this is a test, from 18:00 to 19:00, because I have no announcements from the api.", "Delay", "MetroRail", DateTime.Now, DateTime.Now.AddHours(1), null, new List<string>() { "Bus" }));

                this.Announcements = tempAnnouncements;
            }
#endif
        }

        public string AnnouncementDetails
        {
            get
            {
                string announcementDetails = String.Empty;
                foreach (Announcement announcement in this.Announcements)
                {
                    if (!String.IsNullOrEmpty(announcementDetails))
                        announcementDetails +=  "\n\n";
                    announcementDetails += announcement.Description;
                }
                return announcementDetails;
            }
        }

        public string StartTimeDisplay
        {
            get
            {
                return StartTime.ToLocalTime().ToString("HH:mm");
            }
        }

        public string EndTimeDisplay
        {
            get
            {
                return EndTime.ToLocalTime().ToString("HH:mm");
            }
        }

        public string EndPointDisplay
        {
            get
            {
                return EndStopPoint.Name;
            }
        }

        public string StartPointDisplay
        {
            get
            {
                return StartStopPoint.Name;
            }
        }

        public string StagePointTotal
        {
            get
            {
                return (this.StagePoints.Count() - 2).ToString();
            }
        }

        public Visibility NotPedestrianVisbility
        {
            get
            {
                if (this.Mode.ApplicationTransportMode == Enums.ApplicationTransportMode.Pedestrian)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public Visibility PedestrianVisbility
        {
            get
            {
                if (this.Mode.ApplicationTransportMode == Enums.ApplicationTransportMode.Pedestrian)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility StagePointCountVisibility
        {
            get
            {
                if (this.StagePoints.Count() > 2 && this.Mode.ApplicationTransportMode != Enums.ApplicationTransportMode.Pedestrian)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility AnnouncementVisibility
        {
            get
            {
                if (this.Announcements.Count() > 0)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility NoWaitingStageVisibility
        {
            get
            {
                if (this.WaitingStage == null)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        public Visibility WaitingStageVisibility
        {
            get
            {
                if (this.WaitingStage == null)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        /*
        private void ShowHideAnnouncement()
        {
            if (AnnouncementDetailsVisibility == Visibility.Visible)
            {
                AnnouncementDetailsVisibility = Visibility.Collapsed;
                StopsAndDirectionsVisibility = Visibility.Visible;
            }
            else
            {
                AnnouncementDetailsVisibility = Visibility.Visible;
                StopsAndDirectionsVisibility = Visibility.Collapsed;
            }
        }

        private void ExpandIntermediateDetails()
        {
            if (this.StagePoints.Count() == 2 || this.Mode.ApplicationTransportMode == Enums.ApplicationTransportMode.Pedestrian)
                return;

            if (IntermediateStopVisibility == Visibility.Collapsed)
            {
                ExpandIntermediateStopsUpVisibility = Visibility.Visible;
                ExpandIntermediateStopsDownVisibility = Visibility.Collapsed;
                IntermediateStopVisibility = Visibility.Visible;
            }
            else
            {
                ExpandIntermediateStopsUpVisibility = Visibility.Collapsed;
                ExpandIntermediateStopsDownVisibility = Visibility.Visible;
                IntermediateStopVisibility = Visibility.Collapsed;
            }
        }
        
        private Visibility expandIntermediateStopsDownVisibility = Visibility.Visible;
        public Visibility ExpandIntermediateStopsDownVisibility
        {
            set
            {
                expandIntermediateStopsDownVisibility = value;
                RaisePropertyChanged("ExpandIntermediateStopsDownVisibility");
            }
            get
            {
                return expandIntermediateStopsDownVisibility;
            }
        }

        private Visibility expandIntermediateStopsUpVisibility = Visibility.Collapsed;
        public Visibility ExpandIntermediateStopsUpVisibility
        {
            set
            {
                expandIntermediateStopsUpVisibility = value;
                RaisePropertyChanged("ExpandIntermediateStopsUpVisibility");
            }
            get
            {
                return expandIntermediateStopsUpVisibility;
            }
        }

        private Visibility intermediateStopVisibility = Visibility.Collapsed;
        public Visibility IntermediateStopVisibility
        {
            set
            {
                intermediateStopVisibility = value;
                RaisePropertyChanged("IntermediateStopVisibility");
            }
            get
            {
                return intermediateStopVisibility;
            }
        }

        private Visibility stopsAndDirectionsVisibility = Visibility.Visible;
        public Visibility StopsAndDirectionsVisibility
        {
            set
            {
                stopsAndDirectionsVisibility = value;
                RaisePropertyChanged("StopsAndDirectionsVisibility");
            }
            get
            {
                return stopsAndDirectionsVisibility;
            }
        }

        private Visibility announcementDetailsVisibility = Visibility.Collapsed;
        public Visibility AnnouncementDetailsVisibility
        {
            set
            {
                announcementDetailsVisibility = value;
                RaisePropertyChanged("AnnouncementDetailsVisibility");
            }
            get
            {
                return announcementDetailsVisibility;
            }
        }

        public RelayCommand ExpandIntermediateDetailsCommand
        {
            get { return new RelayCommand(ExpandIntermediateDetails); }
        }

        public RelayCommand ShowHideAnnouncementCommand
        {
            get { return new RelayCommand(ShowHideAnnouncement); }
        }
        */
    }
}
