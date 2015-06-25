using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Messages.Classes;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using Microsoft.Phone.Shell;
using DrumbleApp.Shared.Messages.Enums;
using System.Device.Location;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;
using System.Collections.Generic;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class TripDetailsViewModel : AnalyticsBase, IDisposable
    {
        private DispatcherTimer timer;
        private IBumbleApiService BumbleApi;
        private TripResultsModel tripResultsModel;

        public TripDetailsViewModel(IAggregateService aggregateService, IBumbleApiService BumbleApi)
            : base(ApplicationPage.TripDetails, aggregateService)
        {
            this.BumbleApi = BumbleApi;

            this.timer = new DispatcherTimer();
            this.timer.Tick += TimerTick;

            Messenger.Default.Register<PageLoadMessage>(this, (action) => SendTripMessage(action));
        }

        #region Local Functions

        private void CenterUserLocation()
        {
            if (!base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value)
            {
                ShowPopup(CustomPopupMessageType.Error, AppResources.LocationAppDisabledErrorMessage, AppResources.CustomPopupGenericOkGotItMessage, null);
            }
            else
            {
                if (!user.IsLocationUptodate)
                    base.RegisterWatcher();
                else
                {
                    UserLocationFound(new GpsWatcherResponseMessage(true, user.LastKnownGeneralLocation, GpsWatcherResponseMessageReason.Coordinate));
                }
            }
        }

        private void PinTrip()
        {
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("/Views/TripDetails.xaml?tripId=" + PathResultsModel.SelectedPathOption.TripId.ToString()));

            if (tile == null)
            {
                IEnumerable<Path> paths = UnitOfWork.PathRepository.GetAllCached(UnitOfWork.PublicTransportOperatorRepository.GetAll());

                Path pinnedPath = paths.Where(x => x.TripId == PathResultsModel.SelectedPathOption.TripId).FirstOrDefault();

                if (pinnedPath == null)
                    return;

                pinnedPath.IsPinned = true;
                UnitOfWork.PathRepository.Insert(pinnedPath);
                UnitOfWork.Save();

                var newTile = new StandardTileData()
                {
                    Title = string.Format(AppResources.TripDetailsLiveTileText, PathResultsModel.Destination.ShortAddressText),
                    BackgroundImage = new Uri("/Images/Tiles/TileTrip7.png", UriKind.Relative),
                    BackContent = string.Format(AppResources.TripDetailsLiveTileBackText, PathResultsModel.SelectedPathOption.StartTime.Value.ToString("HH:mm"), PathResultsModel.Location.ShortAddressText, PathResultsModel.Destination.ShortAddressText)
                     
                };

                ShellTile.Create(new Uri("/Views/TripDetails.xaml?tripId=" + PathResultsModel.SelectedPathOption.TripId.ToString(), UriKind.Relative), newTile);
            }
            else
            {
                ShowPopup(CustomPopupMessageType.Error, AppResources.TileAlreadyPinnedErrorText, AppResources.CustomPopupGenericOkGotItMessage, null);
            }
        }

        private void HideApplicationBar()
        {
            ApplicationBarIsVisibile = false;

            base.InMemoryApplicationSettingModel.UpdateSetting(ApplicationSetting.ShowTripApplicationBar, false);
        }

        private void SwitchToList()
        {
            SelectedListItemVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Visible;

        }
        private void SwitchToMap()
        {
            SelectedListItemVisibility = Visibility.Visible;
            ListVisibility = Visibility.Collapsed;
        }

        private void SendTripMessage(PageLoadMessage page)
        {
            if (page.Page == ApplicationPage.TripDetails)
            {
                PathResultsModel.SetSelectedPathOption(PathResultsModel.SelectedPathOption.Letter);
            }
        }

        private void PreviousButton()
        {
            PathResultsModel.PreviousPathOption();
        }
        private void NextButton()
        {
            PathResultsModel.NextPathOption();
        }

        private void PreviousStage()
        {
            PathResultsModel.SelectedPathOption.PreviousStage();
        }
        private void NextStage()
        {
            PathResultsModel.SelectedPathOption.NextStage();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            this.timer.Interval = new TimeSpan(0, 1, 0); 
            PathResultsModel.SelectedPathOption.UpdateDepartureAndArrivalTimes();
        }

        private void FlickPathItems(FlickMessage flickMessage)
        {
            if (flickMessage.ControlName == "MainList")
            {
                switch (flickMessage.Reason)
                {
                    case Messages.Enums.FlickMessageReason.FlickedLeft:
                        NextButton();
                        break;
                    case Messages.Enums.FlickMessageReason.FlickedRight:
                        PreviousButton();
                        break;
                }
            }
            else if (flickMessage.ControlName == "BottomList")
            {
                switch (flickMessage.Reason)
                {
                    case Messages.Enums.FlickMessageReason.FlickedLeft:
                        NextStage();
                        break;
                    case Messages.Enums.FlickMessageReason.FlickedRight:
                        PreviousStage();
                        break;
                    case Messages.Enums.FlickMessageReason.FlickedDown:
                        PreviousStage();
                        break;
                    case Messages.Enums.FlickMessageReason.FlickedUp:
                        NextStage();
                        break;
                }
            }
            else if (flickMessage.ControlName == "MapToggle")
            {
                switch (flickMessage.Reason)
                {
                    case FlickMessageReason.FlickedDown:
                        SwitchToMap();
                        break;
                    case FlickMessageReason.FlickedUp:
                        SwitchToList();
                        break;
                }
            }
        }

        #endregion

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            if (base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowTripApplicationBar).Value)
                ApplicationBarIsVisibile = true;
            else
                ApplicationBarIsVisibile = false;

            Messenger.Default.Register<FlickMessage>(this, (action) => FlickPathItems(action));

            PageTitleMessage.Send(AppResources.HeaderTripDetails);

            PathResultsModel = SimpleIoc.Default.GetInstance<PathResultsModel>();
            PathResultsModel.SelectedRouteStops.Clear();

            if (PathResultsModel.SelectedPathOption == null)
                PathResultsModel.SetSelectedPathOption("A");

            this.timer.Interval = new TimeSpan(0, 0, 60 - DateTime.Now.Second);
            this.timer.Start();

            tripResultsModel = SimpleIoc.Default.GetInstance<TripResultsModel>();
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            PathResultsModel.SelectedRouteStops.Clear();

            Messenger.Default.Unregister<FlickMessage>(this);

            this.timer.Stop();
        }

        protected override void UserLocationFound(GpsWatcherResponseMessage gpsWatcherResponseMessage)
        {
            if (gpsWatcherResponseMessage.Reason != Messages.Enums.GpsWatcherResponseMessageReason.Error)
            {
                if (SentCoordinateRequest || gpsWatcherResponseMessage.IsUsingLastKnown)
                {
                    MapCenterPoint = gpsWatcherResponseMessage.Coordinate.GeoCoordinate;
                    MapZoomLevel = 15;
                    UserLocation = gpsWatcherResponseMessage.Coordinate.GeoCoordinate;
                    UserLocationVisibility = Visibility.Visible;
                }
            }

            base.UserLocationFound(gpsWatcherResponseMessage);
        }

        #endregion

        #region Properties

        private bool applicationBarIsVisibile = false;
        public bool ApplicationBarIsVisibile
        {
            get { return applicationBarIsVisibile; }
            set
            {
                applicationBarIsVisibile = value;
                RaisePropertyChanged("ApplicationBarIsVisibile");
            }
        }

        private PathResultsModel pathResultsModel;
        public PathResultsModel PathResultsModel
        {
            get { return pathResultsModel; }
            set
            {
                pathResultsModel = value;
                pathResultsModel.SelectedRouteStops.Clear();
                RaisePropertyChanged("PathResultsModel");
            }
        }

        // TODO: Config this:
        private GeoCoordinate mapCenterPoint = new GeoCoordinate(-29.9930022, 25.2465820);
        public GeoCoordinate MapCenterPoint
        {
            get { return mapCenterPoint; }
            set
            {
                mapCenterPoint = value;

                RaisePropertyChanged("MapCenterPoint");
            }
        }

        private GeoCoordinate userLocation;
        public GeoCoordinate UserLocation
        {
            get { return userLocation; }
            set
            {
                userLocation = value;

                RaisePropertyChanged("UserLocation");
            }
        }

        private int mapZoomLevel = 5;
        public int MapZoomLevel
        {
            get { return mapZoomLevel; }
            set
            {
                mapZoomLevel = value;

                RaisePropertyChanged("MapZoomLevel");
            }
        }
       

        private Visibility listVisibility = Visibility.Visible;
        public Visibility ListVisibility
        {
            get { return listVisibility; }
            set
            {
                listVisibility = value;
                RaisePropertyChanged("ListVisibility");
            }
        }

        private Visibility selectedListItemVisibility = Visibility.Collapsed;
        public Visibility SelectedListItemVisibility
        {
            get { return selectedListItemVisibility; }
            set
            {
                selectedListItemVisibility = value;
                RaisePropertyChanged("SelectedListItemVisibility");
            }
        }

        private Visibility userLocationVisibility = Visibility.Collapsed;
        public Visibility UserLocationVisibility
        {
            get { return userLocationVisibility; }
            set
            {
                userLocationVisibility = value;
                RaisePropertyChanged("UserLocationVisibility");
            }
        }

        #endregion

        #region Commands

        public RelayCommand HideApplicationBarCommand
        {
            get { return new RelayCommand(HideApplicationBar); }
        }

        public RelayCommand PinTripCommand
        {
            get { return new RelayCommand(PinTrip); }
        }

        public RelayCommand SwitchToListCommand
        {
            get { return new RelayCommand(SwitchToList); }

        }
        public RelayCommand SwitchToMapCommand
        {
            get { return new RelayCommand(SwitchToMap); }
        }

        public RelayCommand PreviousButtonCommand
        {
            get { return new RelayCommand(PreviousButton); }
        }

        public RelayCommand NextButtonCommand
        {
            get { return new RelayCommand(NextButton); }
        }

        public RelayCommand CenterUserLocationCommand
        {
            get { return new RelayCommand(CenterUserLocation); }
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
