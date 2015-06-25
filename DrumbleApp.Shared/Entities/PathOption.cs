using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Models;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DrumbleApp.Shared.Infrastructure.Extensions;

namespace DrumbleApp.Shared.Entities
{
    public class PathOption : ViewModelBase
    {
        private const double maxDisplayWidth = 333;
        private bool isWalkingOnly = false;
        private CancellationTokenSource cancellationTokenSource;
        private IUberService uberService;
        private Coordinate location;
        private Coordinate destination;
        private double maxDurationInMinutes;
        private string accessToken;

        public int Order { get; private set; }
        public bool IsUber { get; set; }
        public UberOption UberOption { get; private set; }
        public string Letter { get; set; }
        public Guid TripId { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public string EstimatedTotalCost { get; private set; }
        public int TotalWalkingDistance { get; private set; }
        public int InitialWalkingDistance { get; private set; }
        public IEnumerable<string> FareMessages { get; private set; }
        public string JsonSerializedObject { get; set; }

        public List<Stage> Stages { get; set; }

        private ObservableCollection<StageModel> stageDisplays = new ObservableCollection<StageModel>();
        public ObservableCollection<StageModel> StageDisplays 
        { 
            get
            {
                return stageDisplays;
            }
        }
        
        private ObservableCollection<RouteStop> routeStops = new ObservableCollection<RouteStop>();
        public ObservableCollection<RouteStop> RouteStops
        {
            get { return routeStops; }

        }

        public void UpdateStageDisplay(DateTime scaledStartTime, bool isScaledStartTime, double maxDurationInMinutes, DateTime end)
        {
            if (DateTime.Now > this.Stages.Last().EndTime)
            {
                StageDisplays.Clear();
                StageDisplays.Add(StageModel.Waiting((int)maxDisplayWidth));
                return;
            }

            StageDisplays.Clear();

            double scale = maxDisplayWidth / maxDurationInMinutes;
            double runningTime = 0;

            if (isScaledStartTime)
            {
                scale = (maxDisplayWidth - 60) / maxDurationInMinutes;

                StageDisplays.Add(StageModel.Waiting(60, Visibility.Visible));
            }

            if (this.Stages.First().StartTime < DateTime.Now)
            {
                runningTime = (DateTime.Now - this.Stages.First().StartTime).TotalMinutes;
            }

            //TODO test with walking!
            Stage previousStage = null;

            // Start by adding a stage for time between now and the first stage.
            if (this.Stages.First().StartTime > scaledStartTime)
            {
                double scaledDurationInMinutes = scale * ((this.Stages.First().StartTime - scaledStartTime).TotalMinutes);

                StageDisplays.Add(StageModel.Waiting((int)(scaledDurationInMinutes)));
            }
            // Add the stages as stage models.
            foreach (Stage stage in Stages)
            {
                double scaledDurationInMinutes = 1.0;

                if (previousStage != null && stage.StartTime > previousStage.EndTime)
                {
                    if (runningTime < (stage.StartTime - previousStage.EndTime).TotalMinutes)
                    {
                        scaledDurationInMinutes = scale * ((stage.StartTime - previousStage.EndTime).TotalMinutes - runningTime);

                        StageDisplays.Add(StageModel.Waiting((int)(scaledDurationInMinutes)));

                        runningTime = 0;
                    }
                    else
                    {
                        runningTime -= (stage.StartTime - previousStage.EndTime).TotalMinutes;
                    }
                }

                previousStage = stage;

                if (runningTime < stage.Duration)
                {
                    scaledDurationInMinutes = scale * (stage.Duration - runningTime);

                    StageDisplays.Add(new StageModel(stage.Colour, (int)(scaledDurationInMinutes)));

                    runningTime = 0;
                }
                else
                {
                    runningTime -= stage.Duration;
                }
            }
            // End by adding a stage for time between the end of this path option and the end time of the longest path option.
            if (this.Stages.Last().EndTime < end)
            {
                if (runningTime < (end - this.Stages.Last().EndTime).TotalMinutes)
                {
                    double scaledDurationInMinutes = scale * ((end - this.Stages.Last().EndTime).TotalMinutes - runningTime);

                    StageDisplays.Add(StageModel.Waiting((int)(scaledDurationInMinutes)));

                    runningTime = 0;
                }
                else
                {
                    runningTime -= (end - this.Stages.Last().EndTime).TotalMinutes;
                }
            }

            // Finally add the extra space to fill the bar within the tile if the trip is short.
            if (StageDisplays.Sum(x => x.Width) < maxDisplayWidth)
            {
                StageDisplays.Add(StageModel.Waiting((int)maxDisplayWidth - StageDisplays.Sum(x => x.Width)));
            }
        }
        public void UpdateStageDisplayUber(DateTime startTime, DateTime endTime, double maxDurationInMinutes)
        {
            double scale = maxDisplayWidth / maxDurationInMinutes;

            if (scale <= 0)
                scale = 1;

            // Start by adding a stage for time between now and the first stage.
            if (startTime > DateTime.Now)
            {
                double scaledDurationInMinutes = scale * ((startTime - DateTime.Now).TotalMinutes);

                StageDisplays.Add(StageModel.Waiting((int)(scaledDurationInMinutes)));
            }
            
            // Add the single stage as this is a car trip.
            StageDisplays.Add(new StageModel("#E0E0E4", (int)(scale * (endTime - startTime).TotalMinutes)));

            // Finally add the extra space to fill the bar within the tile if the trip is short.
            if (StageDisplays.Sum(x => x.Width) < maxDisplayWidth)
            {
                StageDisplays.Add(StageModel.Waiting((int)maxDisplayWidth - StageDisplays.Sum(x => x.Width)));
            }
        }
        private void UpdateStagesIncludeWaiting()
        {
            Stage previousStage = null;

            foreach (Stage stage in Stages)
            {
                if (previousStage != null && stage.StartTime > previousStage.EndTime)
                {
                    double waitingTime = (stage.StartTime - previousStage.EndTime).TotalMinutes;

                    previousStage.WaitingStage = new ValueObjects.WaitingStage((int)waitingTime);
                }

                previousStage = stage;
            }
        }

        public string ArrivalTime
        {
            get
            {
                if (IsUber || DateTime.Now > this.Stages.Last().EndTime)
                    return String.Empty;

                if (!isWalkingOnly)
                {
                    string arrivalTime = this.Stages.Last().EndTime.ToString("HH:mm");

                    string tripTime = TimeConverter.MinutesToText((int)(this.Stages.Last().EndTime - DateTime.Now).TotalMinutes);

                    return string.Format(AppResources.TripDetailsArrivingAt, arrivalTime, tripTime);
                }

                return AppResources.TripSelectionLeavingNow;
            }
        }
        public string DepartureTime
        {
            get
            {
                if (IsUber)
                    return String.Empty;

                if (!isWalkingOnly)
                {
                    if (this.Stages.Last().EndTime < DateTime.Now)
                    {
                        return string.Format(AppResources.TripSelectionTripEnded, TimeConverter.MinutesToText((int)(DateTime.Now - this.Stages.Last().EndTime).TotalMinutes));
                    }
                    if (this.Stages.First().StartTime < DateTime.Now)
                    {
                        return string.Format(AppResources.TripSelectionTripStarted, TimeConverter.MinutesToText((int)(DateTime.Now - this.Stages.First().StartTime).TotalMinutes));
                    }
                    string departureTime = TimeConverter.MinutesToText((int)(this.Stages.First().StartTime - DateTime.Now).TotalMinutes);

                    return string.Format(AppResources.TripSelectionLeavingIn, departureTime);
                }

                return AppResources.TripSelectionLeavingNow;
            }
        }

        private string timeDisplay = String.Empty;
        public string TimeDisplay
        {
            get
            {
                return timeDisplay;
            }
            set
            {
                timeDisplay = value;
                RaisePropertyChanged("TimeDisplay");
            }
        }

        private string durationDisplay = String.Empty;
        public string DurationDisplay
        {
            get
            {
                if (this.IsUber)
                    return durationDisplay;

                int duration = (int)(this.Stages.Last().EndTime - this.Stages.First().StartTime).TotalMinutes;

                if (this.Stages.Last().EndTime < DateTime.Now)
                    return AppResources.TripSelectionCompleteTrip;

                if (this.Stages.First().StartTime < DateTime.Now)
                {
                    duration -= (int)(DateTime.Now - this.Stages.First().StartTime).TotalMinutes;

                    return string.Format(duration.ToString(CultureInfo.InvariantCulture) + "{0}", AppResources.TripSelectionMinutesRemain);
                }

                return string.Format(duration.ToString(CultureInfo.InvariantCulture) + "{0}", AppResources.TripSelectionMinutes);
            }
            set
            {
                durationDisplay = value;
                RaisePropertyChanged("DurationDisplay");
            }
        }

        public Visibility AnnouncementsVisibility
        {
            get
            {
                if (IsUber)
                    return Visibility.Collapsed;

                if (this.Stages.Any(x => x.Announcements.Count() > 0))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility WarningVisibility
        {
            get
            {
                if (IsUber)
                    return Visibility.Collapsed;

                if (this.Stages.First().StartTime.DayOfYear > DateTime.Now.DayOfYear)
                    return Visibility.Visible;
                else if (this.Stages.Last().EndTime.DayOfYear > DateTime.Now.DayOfYear)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility PathOptionVisibility
        {
            get
            {
                if (IsUber)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }
        public Visibility UberOptionVisibility
        {
            get
            {
                if (IsUber)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        private Visibility uberLoaderVisibility = Visibility.Visible;
        public Visibility UberLoaderVisibility
        {
            get
            {
                return uberLoaderVisibility;
            }
            set
            {
                uberLoaderVisibility = value;
                RaisePropertyChanged("UberLoaderVisibility");
            }
        }

        private Visibility uberDetailsVisibility = Visibility.Collapsed;
        public Visibility UberDetailsVisibility
        {
            get
            {
                return uberDetailsVisibility;
            }
            set
            {
                uberDetailsVisibility = value;
                RaisePropertyChanged("UberDetailsVisibility");
            }
        }

        private Visibility uberRetryVisibility = Visibility.Collapsed;
        public Visibility UberRetryVisibility
        {
            get
            {
                return uberRetryVisibility;
            }
            set
            {
                uberRetryVisibility = value;
                RaisePropertyChanged("UberRetryVisibility");
            }
        }

        private Trip trip;
        public Trip Trip 
        {
            get
            {
                return trip;
            }
            set
            {
                trip = value;
            }
        }
        
        private bool uberIsLoading = true;
        public bool UberIsLoading 
        {
            get
            {
                return uberIsLoading;
            }
            set
            {
                uberIsLoading = value;
                RaisePropertyChanged("UberIsLoading");
            }
        }

        private string uberGetText = AppResources.UberLoadingText;
        public string UberGetText 
        {
            get
            {
                return uberGetText;
            }
            set
            {
                uberGetText = value;
                RaisePropertyChanged("UberGetText");
            }
        }

        private string uberEstimatedCost = String.Empty;
        public string UberEstimatedCost 
        {
            get
            {
                return uberEstimatedCost;
            }
            set
            {
                uberEstimatedCost = value;
                RaisePropertyChanged("UberEstimatedCost");
            }
        }
        
        public string WarningDisplay
        {
            get
            {
                if (IsUber)
                    return String.Empty;

                bool hasLongWait = false;
                bool arrivesNextDay = false;
                bool departsNextDay = false;

                if (this.Stages.Where(x => x.WaitingStage != null).Any(x => x.WaitingStage.IsLongWait))
                    hasLongWait = true;

                if (this.Stages.First().StartTime.DayOfYear > DateTime.Now.DayOfYear)
                    departsNextDay = true;
                else if (this.Stages.Last().EndTime.DayOfYear > DateTime.Now.DayOfYear)
                    arrivesNextDay = true;

                if (hasLongWait && arrivesNextDay)
                {
                    return AppResources.TripSelectionLongWaitArrivingNextDayWarning;
                }
                else if (hasLongWait && departsNextDay)
                {
                    return AppResources.TripSelectionLongWaitDepartNextDayWarning;
                }
                else if (hasLongWait)
                {
                    return AppResources.TripSelectionLongWaitWarning;
                }
                else if (arrivesNextDay)
                {
                    return AppResources.TripSelectionArriveNextDayWarning;
                }
                else if (departsNextDay)
                {
                    return AppResources.TripSelectionDepartNextDayWarning;
                }

                return String.Empty;
            }
        }
        public string AnnouncementCountDisplay
        {
            get
            {
                if (IsUber)
                    return String.Empty;

                if (this.Stages.Any(x => x.Announcements.Count() > 0))
                    return this.Stages.Sum(x => x.Announcements.Count()).ToString();

                return String.Empty;
            }
        }
        
        public PathOption(int order, Guid tripId, DateTime? startTime, DateTime? endTime, string estimatedTotalCost, int totalWalkingDistance, int initialWalkingDistance, IEnumerable<Stage> stages, IEnumerable<string> fareMessages, string jsonSerializedObject)
        {
            this.Order = order;
            this.IsUber = false;
            this.EstimatedTotalCost = estimatedTotalCost;
            this.TripId = tripId;
            if (startTime.HasValue) 
                this.StartTime = startTime.Value.ToLocalTime();
            if (endTime.HasValue)
                this.EndTime = endTime.Value.ToLocalTime();
            this.TotalWalkingDistance = totalWalkingDistance;
            this.InitialWalkingDistance = initialWalkingDistance;
            this.Stages = stages.ToList();
            this.StageDisplays.Add(StageModel.Waiting((int)maxDisplayWidth));
            this.JsonSerializedObject = jsonSerializedObject;

            this.RouteStops.AddRange(this.Stages.Where(x => x.Mode.ApplicationTransportMode != ApplicationTransportMode.Pedestrian).SelectMany(x => x.StagePoints));

            UpdateStagesIncludeWaiting();

            this.FareMessages = fareMessages;

            if (this.Stages.Count() == 1 && this.Stages.First().Mode.ApplicationTransportMode == Enums.ApplicationTransportMode.Pedestrian)
                isWalkingOnly = true;

            SetLetter(order);

            string startTimeDisplay = this.Stages.First().StartTime.ToString("HH:mm");
            string endTimeDisplay = (this.Stages.Any(x => x.Mode.ApplicationTransportMode != ApplicationTransportMode.Pedestrian)) ? this.Stages.Where(x => x.Mode.ApplicationTransportMode != ApplicationTransportMode.Pedestrian).Last().EndTime.ToString("HH:mm") : this.Stages.Last().EndTime.ToString("HH:mm");

            this.TimeDisplay = string.Format(AppResources.TripSelectionTimeDisplay, startTimeDisplay, endTimeDisplay);
        }

        /// <summary>
        /// Add an Uber Path option by calling this constructor
        /// </summary>
        public PathOption(int order, IUberService uberService, Coordinate location, Coordinate destination, double maxDurationInMinutes, string accessToken)
        {
            this.uberService = uberService;
            this.location = location;
            this.destination = destination;
            this.maxDurationInMinutes = maxDurationInMinutes;
            IsUber = true;
            this.accessToken = accessToken;

            SetLetter(order);
        }

        private void SetLetter(int order)
        {
            switch (order)
            {
                case 0:
                    this.Letter = "A";
                    break;
                case 1:
                    this.Letter = "B";
                    break;
                case 2:
                    this.Letter = "C";
                    break;
                case 3:
                    this.Letter = "D";
                    break;
                case 4:
                    this.Letter = "E";
                    break;
                case 5:
                    this.Letter = "F";
                    break;
                case 6:
                    this.Letter = "G";
                    break;
            }
        }

        public void UpdateUberOption(UberOption uberOption, double maxDurationInMinutes)
        {
            UberIsLoading = false;
            this.UberOption = uberOption;

            if (uberOption == null)
            {
                UberGetText = AppResources.UberNotFoundText;
                UberLoaderVisibility = Visibility.Collapsed;
                UberDetailsVisibility = Visibility.Collapsed;
                UberRetryVisibility = Visibility.Visible;
                return;
            }

            UberGetText = string.Format(AppResources.UberGetAnUberText, TimeConverter.SecondsToText(uberOption.TimeEstimateInSeconds));
            if (uberOption.LowEstimate != uberOption.HighEstimate)
                UberEstimatedCost = uberOption.CurrencyCode + " " + uberOption.LowEstimate + "-" + uberOption.HighEstimate;
            else
                UberEstimatedCost = uberOption.PriceEstimate;

            DateTime startTime = DateTime.Now.AddSeconds(uberOption.TimeEstimateInSeconds);
            DateTime endTime = startTime.AddSeconds(uberOption.DurationInSeconds);

            this.TimeDisplay = string.Format(AppResources.TripSelectionTimeDisplay, startTime.ToString("HH:mm"), endTime.ToString("HH:mm"));

            int duration = (int)(endTime - startTime).TotalMinutes;
            this.DurationDisplay = string.Format(duration.ToString(CultureInfo.InvariantCulture) + "{0}", AppResources.TripSelectionMinutes);

            this.UpdateStageDisplayUber(startTime, endTime, maxDurationInMinutes);

            UberLoaderVisibility = Visibility.Collapsed;
            UberRetryVisibility = Visibility.Collapsed;
            UberDetailsVisibility = Visibility.Visible;
        }

        public void UpdateDepartureAndArrivalTimes()
        {
            RaisePropertyChanged("DepartureTime");
            RaisePropertyChanged("ArrivalTime");
        }

        public void UpdateTileTimes()
        {
            RaisePropertyChanged("DurationDisplay");
            RaisePropertyChanged("DepartureTime");
        }

        public void NextStage()
        {
            int numStages = Stages.Count();

            if (numStages == 1)
                return;

            for (int i = 0; i < numStages; i++)
            {
                if (Stages[i].Order == SelectedStage.Order)
                {
                    if (i == numStages)
                        SelectedStage = Stages[0];
                    else
                        SelectedStage = Stages[(i + 1) % (numStages)];

                    break;
                }
            }
        }

        public void PreviousStage()
        {
            int numStages = Stages.Count();

            if (numStages == 1)
                return;

            for (int i = 0; i < numStages; i++)
            {
                if (Stages[i].Order == SelectedStage.Order)
                {
                    if (i == 0)
                        SelectedStage = Stages.Last();
                    else
                        SelectedStage = Stages[(i - 1) % (numStages)];

                    break;
                }
            }
        }

        private Stage selectedStage;
        public Stage SelectedStage
        {
            get { return selectedStage; }
            set
            {
                selectedStage = value;
                RaisePropertyChanged("SelectedStage");
            }
        }

        #region Local Functions

        private void UberRetry()
        {
            UberRefresh();
        }

        private void UberFakeSurge()
        {
            Action uberFakeSurge = async () =>
            {
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();

                    string uberOption = await uberService.PutUberProduct(cancellationTokenSource.Token, accessToken, UberOption.ProductId, 2.2);

                }
                catch (Exception)
                {
                    this.UpdateUberOption(null, 0);
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(uberFakeSurge);
        }

        private void UberRefresh()
        {
            UberGetText = AppResources.UberLoadingText;

            UberIsLoading = true;
            UberLoaderVisibility = Visibility.Visible;
            UberDetailsVisibility = Visibility.Collapsed;
            UberRetryVisibility = Visibility.Collapsed;

            UpdateUberTrip(true);
        }

        private void UberCancel()
        {
            UberGetText = AppResources.UberNotFoundText;

            UberIsLoading = false;
            UberLoaderVisibility = Visibility.Collapsed;
            UberDetailsVisibility = Visibility.Collapsed;
            UberRetryVisibility = Visibility.Visible;

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();
        }

        public void UpdateUberTrip(bool refresh)
        {
            if (this.IsUber
                && (this.UberOption == null
                || refresh))
            {
                Action getUberOption = async () =>
                {
                    try
                    {
                        cancellationTokenSource = new CancellationTokenSource();

                        UberOption uberOption = await uberService.GetUberOption(cancellationTokenSource.Token, location, destination);

                        if (uberOption != null)
                        {
                            if (maxDurationInMinutes == 0)
                            {
                                maxDurationInMinutes = new TimeSpan(0, 0, uberOption.DurationInSeconds + uberOption.TimeEstimateInSeconds).TotalMinutes;
                            }
                            this.UpdateUberOption(uberOption, maxDurationInMinutes);
                        }
                        else
                        {
                            this.UpdateUberOption(null, 0);
                        }
                    }
                    catch (Exception)
                    {
                        this.UpdateUberOption(null, 0);
                    }
                };

                DispatcherHelper.CheckBeginInvokeOnUI(getUberOption);
            }
        }

        #endregion

        #region Commands

        public RelayCommand UberRetryCommand
        {
            get { return new RelayCommand(UberRetry); }
        }

        public RelayCommand UberFakeSurgeCommand
        {
            get { return new RelayCommand(UberFakeSurge); }
        }

        public RelayCommand UberCancelCommand
        {
            get { return new RelayCommand(UberCancel); }
        }

        public RelayCommand UberRefreshCommand
        {
            get { return new RelayCommand(UberRefresh); }
        }

        #endregion
    }
}
