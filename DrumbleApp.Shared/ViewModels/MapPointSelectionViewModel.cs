using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Models;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using System.Device.Location;
using System.Threading;
using GalaSoft.MvvmLight.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class MapPointSelectionViewModel : AnalyticsBase, IDisposable
    {
        private SearchType searchType;
        private IBumbleApiService BumbleApiService;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MapPointSelectionViewModel(IAggregateService aggregateService, IBumbleApiService BumbleApiService)
            : base(ApplicationPage.MapPointSelection, aggregateService)
        {
            this.BumbleApiService = BumbleApiService;
        }

        #region Overides

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

        protected override void PageLoaded()
        {
            base.PageLoaded();

            Register();

            searchType = SimpleIoc.Default.GetInstance<SearchTypeModel>().SearchType;

            if (searchType == SearchType.Location)
                PageTitleMessage.Send(AppResources.HeaderChooseLocation);
            else
                PageTitleMessage.Send(AppResources.HeaderChooseDestination);

            if (SimpleIoc.Default.IsRegistered<SearchItem>())
            {
                SearchItem searchItem = SimpleIoc.Default.GetInstance<SearchItem>();

                MapCenterPoint = searchItem.Location.GeoCoordinate;
                MapZoomLevel = 15;
                Point = searchItem.Location.GeoCoordinate;
                SelectedCustomPoint = new Entities.PlaceOfInterest(searchItem.Name, null, searchItem.Description, searchItem.Location, (int) searchItem.Distance);

                PointVisibility = Visibility.Visible;
                FooterBarVisibility = Visibility.Visible;

                SimpleIoc.Default.Unregister<SearchItem>();
            }
            else
            {
                PointVisibility = Visibility.Collapsed;
                FooterBarVisibility = Visibility.Collapsed;

                if (this.user.LastKnownGeneralLocation.IsValid())
                {
                    MapCenterPoint = this.user.LastKnownGeneralLocation.GeoCoordinate;
                    MapZoomLevel = 15;
                    UserLocation = this.user.LastKnownGeneralLocation.GeoCoordinate;
                    UserLocationVisibility = Visibility.Visible;
                }
            }
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Deregister();
        }

        #endregion

        #region Local Static Functions

        private static void GoToSettings()
        {
           // Messenger.Default.Send<Popup>(Popup.Settings);
        }

        #endregion

        #region Local Functions

        private void SetPoint(MapPointMessage message)
        {
            ShowHeaderLoader();

            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            Point = message.Point.GeoCoordinate;
            PointVisibility = Visibility.Visible;

            FooterBarVisibility = Visibility.Collapsed;

            Action search = async () =>
            {
                try
                {
                    Address customAddress = await BumbleApiService.ReverseGeoCode(cancellationTokenSource.Token, user, message.Point);

                    SelectedCustomPoint = new Entities.PlaceOfInterest(customAddress.AddressText, null, customAddress.ShortAddressText, message.Point, -1);

                    FooterBarVisibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    if (ex.Message != AppResources.ApiErrorTaskCancelled)
                    {
                        base.ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);
                    }
                }

                HideHeaderLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(search);
        }

        private void Register()
        {
            Messenger.Default.Register<MapPointMessage>(this, (action) => SetPoint(action));
        }

        private void Deregister()
        {
            Messenger.Default.Unregister<MapPointMessage>(this);
        }

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

        private void Reset()
        {
            PointVisibility = Visibility.Collapsed;
        }

        private void Submit()
        {
            WhereToMessage.Send(new Models.WhereToModel(SelectedCustomPoint.Address, SelectedCustomPoint.Location, searchType));

            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }

        private void Cancel()
        {
            NavigationService.GoBack();
        }
        
        #endregion

        #region Properties

        private Entities.PlaceOfInterest selectedCustomPoint;
        public Entities.PlaceOfInterest SelectedCustomPoint
        {
            get { return selectedCustomPoint; }
            set
            {
                selectedCustomPoint = value;
                RaisePropertyChanged("SelectedCustomPoint");
            }
        }

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
        
        private Visibility footerBarVisibility = Visibility.Collapsed;
        public Visibility FooterBarVisibility
        {
            get { return footerBarVisibility; }
            set
            {
                footerBarVisibility = value;
                RaisePropertyChanged("FooterBarVisibility");
            }
        }

        private Visibility pointVisibility = Visibility.Collapsed;
        public Visibility PointVisibility
        {
            get { return pointVisibility; }
            set
            {
                pointVisibility = value;
                RaisePropertyChanged("PointVisibility");
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


        private GeoCoordinate point;
        public GeoCoordinate Point
        {
            get { return point; }
            set
            {
                point = value;
                RaisePropertyChanged("Point");
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

        #endregion

        #region Commands

        public RelayCommand CenterUserLocationCommand
        {
            get { return new RelayCommand(CenterUserLocation); }
        }

        public RelayCommand CancelCommand
        {
            get { return new RelayCommand(Cancel); }
        }

        public RelayCommand SubmitCommand
        {
            get { return new RelayCommand(Submit); }
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
