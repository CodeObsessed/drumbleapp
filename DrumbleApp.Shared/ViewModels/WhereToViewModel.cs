using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Models;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Linq;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;

namespace DrumbleApp.Shared.ViewModels
{
    public class WhereToViewModel : AnalyticsBase, IDisposable
    {
        private PredefinedDepartureTime selectedPredefinedDepartureTime;
        private DateTime? departureTimeCustom;
        private bool isDeparting = true;
        private IBumbleApiService BumbleApiService;
        private CancellationTokenSource cancellationTokenSource;
        private IUberService uberService;

        public WhereToViewModel(IAggregateService aggregateService, IBumbleApiService BumbleApiService, IUberService uberService)
            : base(ApplicationPage.WhereTo, aggregateService)
        {
            this.user = UnitOfWork.UserRepository.GetUser();
            this.BumbleApiService = BumbleApiService;
            this.uberService = uberService;

            AppCommandMessage.Send(Messages.Enums.AppCommandMessageReason.RemoveBackEntries);

            Messenger.Default.Register<RecentTripMessage>(this, (action) => SetRecentTrip(action));
            Messenger.Default.Register<FavouriteMessage>(this, (action) => SetFavourite(action));
            Messenger.Default.Register<WhereToMessage>(this, (action) => SetWhereTo(action));

            this.LocationSearchBoxModel = new SearchBoxModel(UnitOfWork, InMemoryApplicationSettingModel, this, SearchType.Location, BumbleApiService, user, NavigationService);
            this.DestinationSearchBoxModel = new SearchBoxModel(UnitOfWork, InMemoryApplicationSettingModel, this, SearchType.Destination, BumbleApiService, user, NavigationService);

            this.LocationSearchBoxModel.Changed += LocationSearchBoxModel_Changed;
            this.DestinationSearchBoxModel.Changed += LocationSearchBoxModel_Changed;

            this.LocationSearchBoxModel.SetAutoPopulateState();
        }

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderWhereTo);

            this.LocationSearchBoxModel.Deregister();
            this.DestinationSearchBoxModel.Deregister();

            CanDrumble();

            SetContinueLastTripVisibility();

            Messenger.Default.Unregister<DepartureTimeMessage>(this);
        }

        protected override void UserLocationFound(GpsWatcherResponseMessage gpsWatcherResponseMessage)
        {
            base.UserLocationFound(gpsWatcherResponseMessage);

            if (gpsWatcherResponseMessage.Reason == GpsWatcherResponseMessageReason.Error)
            {
                this.LocationSearchBoxModel.GpsError();
                this.DestinationSearchBoxModel.GpsError();
            }
            else
            {
                this.LocationSearchBoxModel.UserLocationFound(gpsWatcherResponseMessage.Coordinate);
                this.DestinationSearchBoxModel.UserLocationFound(gpsWatcherResponseMessage.Coordinate);
            }
        } 

        #endregion

        #region Local Functions

        private void SetContinueLastTripVisibility()
        {
            bool continueTrip = UnitOfWork.PathRepository.ExistsCached();
            bool continueUber = UnitOfWork.UberTripRepository.ExistsCached();

            if (continueUber)
            {
                if (UnitOfWork.UberTripRepository.GetAll().FirstOrDefault().CreatedDate > DateTime.Now.AddHours(2))
                {
                    UnitOfWork.UberTripRepository.RemoveAll();
                    UnitOfWork.Save();

                    continueUber = false;
                }
            }

            if (continueTrip && continueUber)
            {
                // Both uber and trip available to continue.
                ContinuePreviousTripText = AppResources.WhereToContinueTripButtonText;
                ContrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
                UnSelectedContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
                ContinuePreviousTripVisibility = Visibility.Visible;
                ContinuePreviousTripSecondOptionVisibility = Visibility.Visible;
            }
            else if (continueTrip && !continueUber)
            {
                ContinuePreviousTripText = AppResources.WhereToContinueTripButtonText;
                ContrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
                UnSelectedContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
                ContinuePreviousTripVisibility = Visibility.Visible;
                ContinuePreviousTripSecondOptionVisibility = Visibility.Collapsed;
            }
            else if (!continueTrip && continueUber)
            {
                ContinuePreviousTripText = AppResources.WhereToContinueUberTripButtonText;
                ContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
                UnSelectedContrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
                ContinuePreviousTripVisibility = Visibility.Visible;
                ContinuePreviousTripSecondOptionVisibility = Visibility.Collapsed;
            }
            else
            {
                ContinuePreviousTripText = AppResources.WhereToContinueTripButtonText;
                ContrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
                UnSelectedContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
                ContinuePreviousTripVisibility = Visibility.Collapsed;
                ContinuePreviousTripSecondOptionVisibility = Visibility.Collapsed;
            }
        }

        private void SetFavourite(FavouriteMessage favouriteMessage)
        {
            switch (favouriteMessage.Reason)
            {
                case Messages.Enums.FavouriteMessageReason.SetAsWhereTo:
                    if (cancellationTokenSource != null)
                        cancellationTokenSource.Cancel();
                    LoadFavourite(favouriteMessage.Favourite.Text, favouriteMessage.Favourite.Point, SearchType.Destination);
                    break;
            }
        }

        private void SetRecentTrip(RecentTripMessage recentTripMessage)
        {
            switch (recentTripMessage.Reason)
            {
                case Messages.Enums.RecentTripMessageReason.SetAsWhereToDestination:
                    if (cancellationTokenSource != null)
                        cancellationTokenSource.Cancel();
                    LoadFavourite(recentTripMessage.RecentTrip.Text, recentTripMessage.RecentTrip.Point, SearchType.Destination);
                    break;
                case Messages.Enums.RecentTripMessageReason.SetAsWhereToLocation:
                    if (cancellationTokenSource != null)
                        cancellationTokenSource.Cancel();
                    LoadFavourite(recentTripMessage.RecentTrip.Text, recentTripMessage.RecentTrip.Point, SearchType.Location);
                    break;
            }
        }

        private void SetWhereTo(WhereToMessage whereToMessage)
        {
            if (whereToMessage.WhereToModel.Point != null && whereToMessage.WhereToModel.SearchType == SearchType.Location)
                this.LocationSearchBoxModel.LoadFavourite(whereToMessage.WhereToModel.Text, whereToMessage.WhereToModel.Point);

            else if (whereToMessage.WhereToModel.Point != null && whereToMessage.WhereToModel.SearchType == SearchType.Destination)
                this.DestinationSearchBoxModel.LoadFavourite(whereToMessage.WhereToModel.Text, whereToMessage.WhereToModel.Point);

            else if (string.IsNullOrEmpty(whereToMessage.WhereToModel.Text) && whereToMessage.WhereToModel.Point == null && whereToMessage.WhereToModel.SearchType == SearchType.Location)
                this.LocationSearchBoxModel.LoadLocation();

            else if (string.IsNullOrEmpty(whereToMessage.WhereToModel.Text) && whereToMessage.WhereToModel.Point == null && whereToMessage.WhereToModel.SearchType == SearchType.Destination)
                this.DestinationSearchBoxModel.LoadLocation();

            CheckDrumbleStatus();
        }

        private void LoadFavourite(string text, Coordinate point, SearchType searchType)
        {
            if (searchType == SearchType.Destination)
                this.DestinationSearchBoxModel.LoadFavourite(text, point);
            else if (searchType == SearchType.Location)
                this.LocationSearchBoxModel.LoadFavourite(text, point);

            CheckDrumbleStatus();
        }

        private void CancelDrumble()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            HideHeaderLoader();
            DrumbleButtonIsEnabled = true;
            DrumbleCancelButtonVisibility = Visibility.Collapsed;
        }

        private void CheckDrumbleStatus()
        {
            //Check status of Drumble button.
            if (CanDrumble())
            {
                DrumbleReadyButtonVisibility = Visibility.Visible;
                DrumbleButtonVisibility = Visibility.Collapsed;
            }
            else
            {
                DrumbleReadyButtonVisibility = Visibility.Collapsed;
                DrumbleButtonVisibility = Visibility.Visible;
            }
        }

        private void LocationSearchBoxModel_Changed(object sender, EventArgs e)
        {
            CheckDrumbleStatus();
        }

        private void ChangeToIntervalMode()
        {
            SelectedTimeImageOption = "/Images/64/W/IconTimeInterval.png";
            UnSelectedTimeImageOption = "/Images/64/W/IconDate.png";

            if (isDeparting)
                DepartureTime = string.Format(AppResources.WhereToDepartingText, AppResources.PredefinedDepartureTimeNow);
            else
                DepartureTime = string.Format(AppResources.WhereToArrivingText, AppResources.PredefinedDepartureTimeNow);

            selectedPredefinedDepartureTime = PredefinedDepartureTime.FromNow();
            departureTimeCustom = null;
        }

        private void ChangeToCustomDateMode()
        {
            SelectedTimeImageOption = "/Images/64/W/IconDate.png";
            UnSelectedTimeImageOption = "/Images/64/W/IconTimeInterval.png";

            if (isDeparting)
                DepartureTime = string.Format(AppResources.WhereToDepartingAtText, DateTime.Now.ToString("g"));
            else
                DepartureTime = string.Format(AppResources.WhereToArrivingAtText, DateTime.Now.ToString("g"));

            departureTimeCustom = DateTime.Now;
            selectedPredefinedDepartureTime = null;
        }

        private void ChangeContinuePreviousTrip()
        {
            if (UnSelectedContrinuePreviousTripImageOption == "/Images/64/W/TripKarmaWhite.png")
            {
                UnSelectedContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
                ContrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
                ContinuePreviousTripText = AppResources.WhereToContinueTripButtonText;
            }
            else
            {
                UnSelectedContrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
                ContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
                ContinuePreviousTripText = AppResources.WhereToContinueUberTripButtonText;
            }
        }

        private void ChangeTimeSelectionMode()
        {
            if (UnSelectedTimeImageOption == "/Images/64/W/IconTimeInterval.png")
            {
                ChangeToIntervalMode();
            }
            else
            {
                ChangeToCustomDateMode();
            }
        }
        
        private void DepartureTimeTap()
        {
            Messenger.Default.Register<DepartureTimeMessage>(this, (action) => SetDepartureTime(action));

            if (SelectedTimeImageOption == "/Images/64/W/IconTimeInterval.png")
            {
                NavigationService.NavigateTo("/Views/DateTimeSelection.xaml?pagestate=interval");
            }
            else
            {
                NavigationService.NavigateTo("/Views/DateTimeSelection.xaml?pagestate=custom");
            }
        }

        private void SetDepartureTime(DepartureTimeMessage departureTimeMessage)
        {
            switch (departureTimeMessage.Reason)
            {
                case Messages.Enums.DepartureTimeMessageReason.DateTime:
                    ChangeToCustomDateMode();

                    selectedPredefinedDepartureTime = null;

                    if (departureTimeMessage.IsDeparting)
                        DepartureTime = string.Format(AppResources.WhereToDepartingAtText, departureTimeMessage.DateTime.ToString());
                    else
                        DepartureTime = string.Format(AppResources.WhereToArrivingAtText, departureTimeMessage.DateTime.ToString());

                    departureTimeCustom = departureTimeMessage.DateTime;
                    isDeparting = departureTimeMessage.IsDeparting;

                    break;

                case Messages.Enums.DepartureTimeMessageReason.PreDefined:
                    ChangeToIntervalMode();

                    selectedPredefinedDepartureTime = departureTimeMessage.PredefinedDepartureTime;
                    isDeparting = departureTimeMessage.IsDeparting;

                    if (departureTimeMessage.IsDeparting)
                    {
                        if (departureTimeMessage.PredefinedDepartureTime.DepartureTimeInMinutes == 0)
                            DepartureTime = string.Format(AppResources.WhereToDepartingText, departureTimeMessage.PredefinedDepartureTime.DepartureTime);
                        else
                            DepartureTime = string.Format(AppResources.WhereToDepartingInText, departureTimeMessage.PredefinedDepartureTime.DepartureTime);
                    }
                    else
                    {
                        if (departureTimeMessage.PredefinedDepartureTime.DepartureTimeInMinutes == 0)
                            DepartureTime = string.Format(AppResources.WhereToArrivingText, departureTimeMessage.PredefinedDepartureTime.DepartureTime);
                        else
                            DepartureTime = string.Format(AppResources.WhereToArrivingInText, departureTimeMessage.PredefinedDepartureTime.DepartureTime);
                    }

                    departureTimeCustom = null;

                    break;
            }
            CheckDrumbleStatus();
        }

        private void Switch()
        {
            this.LocationSearchBoxModel.Switch();
            this.DestinationSearchBoxModel.Switch();

            SearchBoxModel tempSearchBoxModel = this.LocationSearchBoxModel;
            this.LocationSearchBoxModel = this.DestinationSearchBoxModel;
            this.DestinationSearchBoxModel = tempSearchBoxModel;
        }

        private bool CanDrumble()
        {
            if (this.LocationSearchBoxModel.IsValid() && this.DestinationSearchBoxModel.IsValid())
            {
                if (departureTimeCustom != null && departureTimeCustom < DateTime.Now)
                    return false;

                if (!UnitOfWork.TransportModeRepository.GetAll().Any(x => x.IsEnabled))
                    return false;

                if (UnitOfWork.OperatorSettingRepository.GetAll().Where(x => x.IsEnabled).Count() == 0)
                    return false;

                return true;
            }

            return false;
        }

        private void Drumble()
        {
            if (!this.LocationSearchBoxModel.IsValid())
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToLocationErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
            }
            else if (!this.DestinationSearchBoxModel.IsValid())
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToDestinationErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
            }
            else if (departureTimeCustom != null && departureTimeCustom < DateTime.Now.AddMinutes(-5))
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToCustomDateErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
            }
            else
            {
                IEnumerable<TransportMode> modes = UnitOfWork.TransportModeRepository.GetAll();

                if (!modes.Any(x => x.IsEnabled))
                {
                    base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToNoModesErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
                    return;
                }

                IEnumerable<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll();

                if (!operatorSettings.Any(x => x.IsEnabled))
                {
                    base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToNoModesErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
                    return;
                }

                DrumbleButtonIsEnabled = false;
                DrumbleCancelButtonVisibility = Visibility.Visible;
                ShowHeaderLoader();

                Action getPath = async () =>
                {
                    try
                    {
                        DateTime createdDate = DateTime.UtcNow;

                        // Try save a recent trip if user allows it.
                        if (base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.StoreRecent).Value)
                        {
                            Recent recentTripLocation = new Recent(this.LocationSearchBoxModel.TripOptions.Location, this.LocationSearchBoxModel.TripOptions.Text, createdDate, createdDate);
                            UnitOfWork.RecentTripRepository.Insert(recentTripLocation);

                            Recent recentTripDestination = new Recent(this.DestinationSearchBoxModel.TripOptions.Location, this.DestinationSearchBoxModel.TripOptions.Text, createdDate, createdDate);
                            UnitOfWork.RecentTripRepository.Insert(recentTripDestination);

                            UnitOfWork.Save();
                        }

                        int? timeOffSetInMinutes = null;
                        DateTime? seletedDepartureDate = null;

                        if (selectedPredefinedDepartureTime != null)
                        {
                            timeOffSetInMinutes = selectedPredefinedDepartureTime.DepartureTimeInMinutes;
                        }
                        else if (departureTimeCustom != null)
                        {
                            seletedDepartureDate = departureTimeCustom.Value.ToUniversalTime();
                        }
                        else
                        {
                            timeOffSetInMinutes = 0;
                        }

                        List<string> excludedModes = modes.Where(x => x.IsEnabled == false).Select(x => x.ApplicationTransportMode.ToString()).ToList();
                        List<string> excludedOperators = operatorSettings.Where(x => x.IsEnabled == false).Select(x => x.OperatorName).ToList();

                        cancellationTokenSource = new CancellationTokenSource();

                        IEnumerable<PathOption> pathOptionResults = await BumbleApiService.Path(cancellationTokenSource.Token, UnitOfWork.UserRepository.GetUser(), this.LocationSearchBoxModel.TripOptions.Location, this.DestinationSearchBoxModel.TripOptions.Location, isDeparting, seletedDepartureDate, timeOffSetInMinutes, excludedModes, excludedOperators);

                        List<PathOption> pathOptions = pathOptionResults.ToList();

                        // Attempt to add an uber option.
                        if (modes.Where(x => x.IsEnabled).Select(x => x.ApplicationTransportMode).Contains(ApplicationTransportMode.Taxi)
                            && base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.UseUber).Value
                            && !UnitOfWork.UberTripRepository.ExistsCached())
                        {
                            double maxDurationInMinutes = (!pathOptions.Any()) ? 0 : (pathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Max(x => x.EndTime) - DateTime.Now).TotalMinutes;
                            pathOptions.Add(new PathOption(pathOptions.Count(), uberService, LocationSearchBoxModel.TripOptions.Location, DestinationSearchBoxModel.TripOptions.Location, maxDurationInMinutes, (user.UberInfo == null) ? null : user.UberInfo.AccessToken));
                        }

                        if (!pathOptions.Any())
                        {
                            base.ShowPopup(CustomPopupMessageType.Information, AppResources.WhereToNoResultsFound, AppResources.CustomPopupGenericOkMessage, null);

                            HideHeaderLoader();
                            DrumbleButtonIsEnabled = true;
                            DrumbleCancelButtonVisibility = Visibility.Collapsed;
                            return;
                        }

                        DateTime startDate = DateTime.Now;
                        UnitOfWork.PathRepository.RemoveAll();
                        UnitOfWork.Save();

                        foreach (PathOption pathOption in pathOptions.Where(x => !x.IsUber))
                        {
                            if (pathOption.EndTime != null)
                                UnitOfWork.PathRepository.Insert(new Path(pathOption.TripId, startDate, pathOption.EndTime.Value, LocationSearchBoxModel.TripOptions.Text, DestinationSearchBoxModel.TripOptions.Text, LocationSearchBoxModel.TripOptions.Location, DestinationSearchBoxModel.TripOptions.Location, false, pathOption.JsonSerializedObject, pathOption.Order));
                        }

                        UnitOfWork.Save();

                        PathResultsModel results = new PathResultsModel(this.LocationSearchBoxModel.TripOptions.Address, this.DestinationSearchBoxModel.TripOptions.Address, pathOptions, BumbleApiService, user);

                        SimpleIoc.Default.Unregister<PathResultsModel>();

                        SimpleIoc.Default.Register<PathResultsModel>(() =>
                        {
                            return results;
                        });

                        SimpleIoc.Default.Unregister<TripResultsModel>();

                        SimpleIoc.Default.Register<TripResultsModel>(() =>
                        {
                            return new TripResultsModel();
                        });

                        if (base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.SkipTripSelection).Value)
                        {
                            this.NavigationService.NavigateTo("/Views/TripDetails.xaml");
                        }
                        else
                        {
                            this.NavigationService.NavigateTo("/Views/TripSelection.xaml");
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "Cancelled")
                            base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                    }

                    HideHeaderLoader();
                    DrumbleButtonIsEnabled = true;
                    DrumbleCancelButtonVisibility = Visibility.Collapsed;
                };

                DispatcherHelper.CheckBeginInvokeOnUI(getPath);
            }
        }

        private void Favourites()
        {
            NavigationService.NavigateTo("/Views/Favourites.xaml?fromwhereto=true&state=favourites");
        }

        private void Recent()
        {
            NavigationService.NavigateTo("/Views/Favourites.xaml?fromwhereto=true&state=recent");
        }

        private void MainMenu()
        {
            NavigationService.NavigateTo("/Views/MainMenu.xaml");
        }

        private void AnnouncementMenu()
        {
            NavigationService.NavigateTo("/Views/Announcements.xaml");
        }

        private void ContinuePreviousTrip()
        {
            if (ContrinuePreviousTripImageOption == "/Images/64/W/TripKarmaWhite.png")
            {
                // Check if we must load last trip from the database first.
                if (!SimpleIoc.Default.ContainsCreated<PathResultsModel>())
                {
                    IEnumerable<Path> paths = UnitOfWork.PathRepository.GetAllCached(UnitOfWork.PublicTransportOperatorRepository.GetAll());

                    List<PathOption> pathOptions = paths.Select(x => x.PathOption).ToList();

                    Path firstPath = paths.First();

                    IEnumerable<TransportMode> modes = UnitOfWork.TransportModeRepository.GetAll();
                    // Attempt to add an uber option.
                    if (modes.Where(x => x.IsEnabled).Select(x => x.ApplicationTransportMode).Contains(ApplicationTransportMode.Taxi)
                        && base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.UseUber).Value
                        && !UnitOfWork.UberTripRepository.ExistsCached())
                    {
                        double maxDurationInMinutes = (pathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Max(x => x.EndTime) - DateTime.Now).TotalMinutes;
                        pathOptions.Add(new PathOption(pathOptions.Count(), uberService, firstPath.Location, firstPath.Destination, maxDurationInMinutes, user.UberInfo.AccessToken));
                    }

                    PathResultsModel results = new PathResultsModel(new Address(firstPath.LocationText, firstPath.LocationText, firstPath.Location), new Address(firstPath.DestinationText, firstPath.DestinationText, firstPath.Destination), pathOptions, BumbleApiService, user);

                    if (SimpleIoc.Default.IsRegistered<PathResultsModel>())
                        SimpleIoc.Default.Unregister<PathResultsModel>();

                    SimpleIoc.Default.Register<PathResultsModel>(() =>
                    {
                        return results;
                    });

                    if (SimpleIoc.Default.IsRegistered<TripResultsModel>())
                        SimpleIoc.Default.Unregister<TripResultsModel>();

                    SimpleIoc.Default.Register<TripResultsModel>(() =>
                    {
                        return new TripResultsModel();
                    });
                }

                if (base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.SkipTripSelection).Value)
                {
                    this.NavigationService.NavigateTo("/Views/TripDetails.xaml");
                }
                else
                {
                    this.NavigationService.NavigateTo("/Views/TripSelection.xaml");
                }
            }
            else
            {
                // Check if uber trip isn't already in memory
                if (!SimpleIoc.Default.ContainsCreated<UberTrip>())
                {
                    UberTrip uberTrip = UnitOfWork.UberTripRepository.GetAll().FirstOrDefault();

                    if (uberTrip != null)
                    {
                        if (SimpleIoc.Default.IsRegistered<UberTrip>())
                            SimpleIoc.Default.Unregister<UberTrip>();

                        SimpleIoc.Default.Register<UberTrip>(() =>
                        {
                            return uberTrip;
                        });

                        this.NavigationService.NavigateTo("/Views/UberTripDetails.xaml");
                    }
                }
                else
                {
                    this.NavigationService.NavigateTo("/Views/UberTripDetails.xaml");
                }
                
            }
        }

        #endregion

        #region Properties

        private SearchBoxModel locationSearchBoxModel;
        public SearchBoxModel LocationSearchBoxModel
        {
            get { return locationSearchBoxModel; }
            set
            {
                locationSearchBoxModel = value;
                RaisePropertyChanged("LocationSearchBoxModel");
            }
        }

        private SearchBoxModel destinationSearchBoxModel;
        public SearchBoxModel DestinationSearchBoxModel
        {
            get { return destinationSearchBoxModel; }
            set
            {
                destinationSearchBoxModel = value;
                RaisePropertyChanged("DestinationSearchBoxModel");
            }
        }

        private string selectedTimeImageOption = "/Images/64/W/IconTimeInterval.png";
        public string SelectedTimeImageOption
        {
            get { return selectedTimeImageOption; }
            set
            {
                selectedTimeImageOption = value;
                RaisePropertyChanged("SelectedTimeImageOption");
            }
        }

        private string unSelectedTimeImageOption = "/Images/64/W/IconDate.png";
        public string UnSelectedTimeImageOption
        {
            get { return unSelectedTimeImageOption; }
            set
            {
                unSelectedTimeImageOption = value;
                RaisePropertyChanged("UnSelectedTimeImageOption");
            }
        }

        private string contrinuePreviousTripImageOption = "/Images/64/W/TripKarmaWhite.png";
        public string ContrinuePreviousTripImageOption
        {
            get { return contrinuePreviousTripImageOption; }
            set
            {
                contrinuePreviousTripImageOption = value;
                RaisePropertyChanged("ContrinuePreviousTripImageOption");
            }
        }

        private string continuePreviousTripText = AppResources.WhereToContinueTripButtonText;
        public string ContinuePreviousTripText
        {
            get { return continuePreviousTripText; }
            set
            {
                continuePreviousTripText = value;
                RaisePropertyChanged("ContinuePreviousTripText");
            }
        }
        
        private string unSelectedContrinuePreviousTripImageOption = "/Images/Uber/Uber.png";
        public string UnSelectedContrinuePreviousTripImageOption
        {
            get { return unSelectedContrinuePreviousTripImageOption; }
            set
            {
                unSelectedContrinuePreviousTripImageOption = value;
                RaisePropertyChanged("UnSelectedContrinuePreviousTripImageOption");
            }
        }

        private string departureTime = string.Format(AppResources.WhereToDepartingText, AppResources.PredefinedDepartureTimeNow);
        public string DepartureTime
        {
            get { return departureTime; }
            set
            {
                departureTime = value;
                RaisePropertyChanged("DepartureTime");
            }
        }

        private Visibility continuePreviousTripVisibility = Visibility.Collapsed;
        public Visibility ContinuePreviousTripVisibility
        {
            get { return continuePreviousTripVisibility; }
            set
            {
                continuePreviousTripVisibility = value;
                RaisePropertyChanged("ContinuePreviousTripVisibility");
            }
        }

        private Visibility continuePreviousTripSecondOptionVisibility = Visibility.Collapsed;
        public Visibility ContinuePreviousTripSecondOptionVisibility
        {
            get { return continuePreviousTripSecondOptionVisibility; }
            set
            {
                continuePreviousTripSecondOptionVisibility = value;
                RaisePropertyChanged("ContinuePreviousTripSecondOptionVisibility");
            }
        }

        private Visibility DrumbleCancelButtonVisibility = Visibility.Collapsed;
        public Visibility DrumbleCancelButtonVisibility
        {
            get { return DrumbleCancelButtonVisibility; }
            set
            {
                DrumbleCancelButtonVisibility = value;
                RaisePropertyChanged("DrumbleCancelButtonVisibility");
            }
        }

        private Visibility DrumbleButtonVisibility = Visibility.Visible;
        public Visibility DrumbleButtonVisibility
        {
            get { return DrumbleButtonVisibility; }
            set
            {
                DrumbleButtonVisibility = value;
                RaisePropertyChanged("DrumbleButtonVisibility");
            }
        }

        private Visibility DrumbleReadyButtonVisibility = Visibility.Collapsed;
        public Visibility DrumbleReadyButtonVisibility
        {
            get { return DrumbleReadyButtonVisibility; }
            set
            {
                DrumbleReadyButtonVisibility = value;
                RaisePropertyChanged("DrumbleReadyButtonVisibility");
            }
        }

        private bool DrumbleButtonIsEnabled = true;
        public bool DrumbleButtonIsEnabled
        {
            get { return DrumbleButtonIsEnabled; }
            set
            {
                DrumbleButtonIsEnabled = value;
                RaisePropertyChanged("DrumbleButtonIsEnabled");
            }
        }
        
        #endregion

        #region Commands

        public RelayCommand ContinuePreviousTripCommand
        {
            get { return new RelayCommand(ContinuePreviousTrip); }

        }
        public RelayCommand FavouritesCommand
        {
            get { return new RelayCommand(Favourites); }
        }

        public RelayCommand RecentCommand
        {
            get { return new RelayCommand(Recent); }
        }
        
        public RelayCommand DrumbleCancelCommand
        {
            get { return new RelayCommand(CancelDrumble); }
        }

        public RelayCommand DrumbleCommand
        {
            get { return new RelayCommand(Drumble); }
        }

        public RelayCommand SwitchCommand
        {
            get { return new RelayCommand(Switch); }
        }

        public RelayCommand DepartureTimeTapCommand
        {
            get { return new RelayCommand(DepartureTimeTap); }
        }

        public RelayCommand ChangeTimeSelectionModeCommand
        {
            get { return new RelayCommand(ChangeTimeSelectionMode); }
        }

        public RelayCommand MainMenuCommand
        {
            get { return new RelayCommand(MainMenu); }
        }

        public RelayCommand AnnouncementMenuCommand
        {
            get { return new RelayCommand(AnnouncementMenu); }
        }

        public RelayCommand ChangeContinuePreviousTripCommand
        {
            get { return new RelayCommand(ChangeContinuePreviousTrip); }
        }

        #endregion

        #region Cleanup

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    UnitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}