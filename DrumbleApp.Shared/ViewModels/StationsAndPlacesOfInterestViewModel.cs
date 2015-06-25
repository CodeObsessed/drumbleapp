using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DrumbleApp.Shared.Infrastructure.Extensions;
using GalaSoft.MvvmLight.Messaging;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using DrumbleApp.Shared.Resources;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using System.Threading.Tasks;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using System.Device.Location;
using System.Threading;
using DrumbleApp.Shared.Models;
using System.Globalization;

namespace DrumbleApp.Shared.ViewModels
{
    public class StationsAndPlacesOfInterestViewModel : AnalyticsBase, IDisposable
    {
        private IBumbleApiService BumbleApiService;
        private DispatcherTimer timerSearch;
        private DispatcherTimer timerMapMoved;
        private bool pageLoaded = false;
        private bool isStopSearch = true;
        private bool isPointA = true;
        private bool fromWhereTo = false;
        private bool locationIsAllowed = false;
        private const int numNearbyToLoadForMap = 40;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public StationsAndPlacesOfInterestViewModel(IAggregateService aggregateService, IBumbleApiService BumbleApiService)
            : base(ApplicationPage.Settings, aggregateService)
        {
            this.timerSearch = new DispatcherTimer();
            this.timerSearch.Interval = new TimeSpan(0, 0, 1);
            this.timerSearch.Tick += TimerTickSearch;

            this.timerMapMoved = new DispatcherTimer();
            this.timerMapMoved.Interval = new TimeSpan(0, 0, 1);
            this.timerMapMoved.Tick += TimerTickMapMoved;

            this.BumbleApiService = BumbleApiService;

            Messenger.Default.Register<StationsAndPlacesOfInterestModeMessage>(this, (action) => SetSelectionMode(action));
        }

        #region Local Functions

        private void PointAClick()
        {
            if (!this.isPointA)
            {
                PointA.Select();

                this.isPointA = true;

                PointB.Deselect();
               
                if (PointA.HasValue())
                {
                    if (PointA.Station == null && PointA.CustomPoint == null)
                    {
                        SelectedSearchItemForView = PointA.SearchItem;

                        SelectedStationVisibility = Visibility.Collapsed;
                        SelectedCustomPointVisibility = Visibility.Collapsed;
                        SelectedSearchItemVisibility = Visibility.Visible;
                    }
                    else if (PointA.SearchItem == null && PointA.CustomPoint == null)
                    {
                        SelectedStationForView = PointA.Station;

                        SelectedSearchItemVisibility = Visibility.Collapsed;
                        SelectedCustomPointVisibility = Visibility.Collapsed;
                        SelectedStationVisibility = Visibility.Visible;
                    }
                    else 
                    {
                        SelectedCustomPointForView = PointA.CustomPoint;

                        SelectedSearchItemVisibility = Visibility.Collapsed;
                        SelectedStationVisibility = Visibility.Collapsed;
                        SelectedCustomPointVisibility = Visibility.Visible;
                    }

                    PointA.Show();
                }
                else
                {
                    PointA.Hide();
                    SelectedStationVisibility = Visibility.Collapsed;
                    SelectedSearchItemVisibility = Visibility.Collapsed;
                    SelectedCustomPointVisibility = Visibility.Collapsed;
                }
            }
        }

        private void PointBClick()
        {
            if (this.isPointA)
            {
                PointB.Select();

                this.isPointA = false;

                PointA.Deselect();

                if (PointB.HasValue())
                {
                    if (PointB.Station == null && PointB.CustomPoint == null)
                    {
                        SelectedSearchItemForView = PointB.SearchItem;

                        SelectedStationVisibility = Visibility.Collapsed;
                        SelectedSearchItemVisibility = Visibility.Visible;
                        SelectedCustomPointVisibility = Visibility.Collapsed;
                    }
                    else if (PointB.SearchItem == null && PointB.CustomPoint == null)
                    {
                        SelectedStationForView = PointB.Station;

                        SelectedSearchItemVisibility = Visibility.Collapsed;
                        SelectedStationVisibility = Visibility.Visible;
                        SelectedCustomPointVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        SelectedCustomPointForView = PointB.CustomPoint;

                        SelectedSearchItemVisibility = Visibility.Collapsed;
                        SelectedStationVisibility = Visibility.Collapsed;
                        SelectedCustomPointVisibility = Visibility.Visible;
                    }

                    PointB.Show();
                }
                else
                {
                    PointB.Hide();
                    SelectedStationVisibility = Visibility.Collapsed;
                    SelectedSearchItemVisibility = Visibility.Collapsed;
                    SelectedCustomPointVisibility = Visibility.Collapsed;
                }
            }
        }

        private void AddStations()
        {
            ShowLoader();
            Stations.Clear();
            SearchResults.Clear();

            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            Stations.AddRange(UnitOfWork.PublicStopRepository.GetNearby(30));

            timerMapMoved.Start();

            ShowStopsList();
        }

        private void AddSearchResultsNearby()
        {
            ShowLoader();
            Stations.Clear();
            SearchResults.Clear();

            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            Action search = async () =>
            {
                try
                {
                    SearchResults.Clear();

                    string[] categories = UnitOfWork.PlaceOfInterestCategoryRepository.GetAll().Where(x => x.Category != "Unknown").Select(x => x.Category).ToArray();

                    // Only search if we have user location
                    if (locationIsAllowed && user.LastKnownGeneralLocation != null && user.LastKnownGeneralLocation.IsValid())
                    {
                        SearchResults.AddRange(await BumbleApiService.PlacesOfInterest(cancellationTokenSource.Token, user, String.Empty, categories));

                        timerMapMoved.Start();
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message != AppResources.ApiErrorTaskCancelled)
                    {
                        base.ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);
                    }
                }

                ShowSearchList();
                HideHeaderLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(search);
        }

        private void SearchTextBoxGotFocus()
        {
            if (SearchText.Equals(AppResources.SearchStationsAndPlacesOfInterestTextBoxWatermark))
            {
                SearchText = String.Empty;
            }
        }

        private void ClearSearchTextBox()
        {
            ShowSearchIcon();

            SearchText = AppResources.SearchStationsAndPlacesOfInterestTextBoxWatermark;
        }

        private void ClearSearch()
        {
            ClearSearchTextBox();
        }

        private void ClearPoints()
        {
            SelectedSearchItemForView = null;
            SelectedStationForView = null;
            SelectedCustomPointForView = null;

            SelectedSearchItemVisibility = Visibility.Collapsed;
            SelectedStationVisibility = Visibility.Collapsed;
            SelectedCustomPointVisibility = Visibility.Collapsed;

            PointA = new WhereToPoint(true);
            PointB = new WhereToPoint(false);
        }

        private void SearchTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                SearchText = AppResources.SearchStationsAndPlacesOfInterestTextBoxWatermark;
            }
        }

        private void ShowStopsList()
        {
            LoaderVisibility = Visibility.Collapsed;
            StopListVisibility = Visibility.Visible;
            SearchListVisibility = Visibility.Collapsed;

            if (Stations.Count() == 0)
            {
                NoResultsVisibility = Visibility.Visible;
                StopListVisibility = Visibility.Collapsed;
            }
            else
            {
                StopListVisibility = Visibility.Visible;
                NoResultsVisibility = Visibility.Collapsed;
            }
        }

        private void ShowSearchList()
        {
            LoaderVisibility = Visibility.Collapsed;
            SearchListVisibility = Visibility.Visible;
            StopListVisibility = Visibility.Collapsed;

            if (SearchResults.Count() == 0)
            {
                NoResultsVisibility = Visibility.Visible;
                SearchListVisibility = Visibility.Collapsed;
            }
            else
            {
                SearchListVisibility = Visibility.Visible;
                NoResultsVisibility = Visibility.Collapsed;
            }
        }

        private void ShowLoader()
        {
            LoaderVisibility = Visibility.Visible;
            StopListVisibility = Visibility.Collapsed;
            SearchListVisibility = Visibility.Collapsed;
        }

        private void ShowSearchIcon()
        {
            ClearSearchImageVisibility = Visibility.Collapsed;
            SearchImageVisibility = Visibility.Visible;
        }

        private void ShowDeleteIcon()
        {
            ClearSearchImageVisibility = Visibility.Visible;
            SearchImageVisibility = Visibility.Collapsed;
        }

        private void SwitchToSearch()
        {
            isStopSearch = false;

            StationsVisibility = Visibility.Collapsed;
            SearchVisibility = Visibility.Visible;

            SelectedStationVisibility = Visibility.Collapsed;
            SelectedCustomPointVisibility = Visibility.Collapsed;
            SelectedSearchItemVisibility = Visibility.Collapsed;

            if (isPointA)
            {
                PointA.Hide();
            }
            else
            {
                PointB.Hide();
            }

            MapStops.Clear();

            ClearSearchTextBox();

            ShowSearchList();
        }

        private void SwitchToStations()
        {
            isStopSearch = true;

            StationsVisibility = Visibility.Visible;
            SearchVisibility = Visibility.Collapsed;

            SelectedStationVisibility = Visibility.Collapsed;
            SelectedCustomPointVisibility = Visibility.Collapsed;
            SelectedSearchItemVisibility = Visibility.Collapsed;

            if (isPointA)
            {
                PointA.Hide();
            }
            else
            {
                PointB.Hide();
            }

            MapSearchResults.Clear();

            ClearSearchTextBox();

            ShowStopsList();
        }

        private void SwitchToList()
        {
            SwitchToMapVisibility = Visibility.Visible;
            MapVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Visible;
        }

        private void SwitchToMap()
        {
            MapVisibility = Visibility.Visible;
            ListVisibility = Visibility.Collapsed;
        }

        private void LoadSearch()
        {
            StationsVisibility = Visibility.Collapsed;
            SearchVisibility = Visibility.Visible;

            isStopSearch = false;
        }

        private void LoadStations()
        {
            StationsVisibility = Visibility.Visible;
            SearchVisibility = Visibility.Collapsed;

            isStopSearch = true;
        }

        private void CenterUserLocation()
        {
            if (!locationIsAllowed)
            {
                Messenger.Default.Send<CustomPopupMessageWithAction>(new CustomPopupMessageWithAction(CustomPopupMessageType.Error, AppResources.LocationAppDisabledErrorMessage, AppResources.CustomPopupGenericOkMessage, AppResources.CustomPopupGenericGoToSettings, null, GoToSettings, null));
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

        private void ReCenterOnPoint()
        {
            if (isPointA)
            {
                if (PointA.Station != null)
                    SelectedStation = PointA.Station;
                else if (PointA.CustomPoint != null)
                    SelectedCustomPoint = PointA.CustomPoint;
                else if (PointA.SearchItem != null)
                    SelectedSearchResult = PointA.SearchItem;
                
            }
            else
            {
                if (PointB.Station != null)
                    SelectedStation = PointB.Station;
                else if (PointB.CustomPoint != null)
                    SelectedCustomPoint = PointB.CustomPoint;
                else if (PointB.SearchItem != null)
                    SelectedSearchResult = PointB.SearchItem;
            }
        }

        private void RemovePointA()
        {
            PointA.ClearValues();

            if (isPointA)
            {
                SelectedStationVisibility = Visibility.Collapsed;
                SelectedSearchItemVisibility = Visibility.Collapsed;
                SelectedCustomPointVisibility = Visibility.Collapsed;
            }
        }

        private void RemovePointB()
        {
            PointB.ClearValues();

            if (!isPointA)
            {
                SelectedStationVisibility = Visibility.Collapsed;
                SelectedSearchItemVisibility = Visibility.Collapsed;
                SelectedCustomPointVisibility = Visibility.Collapsed;
            }
        }

        private void Cancel()
        {
            NavigationService.GoBack();
        }

        private void Submit()
        {
            if (!PointA.HasValue() && !PointB.HasValue())
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.MapPointSelectionNoPointsPlacedPopupText, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            if (fromWhereTo)
            {
                WhereToMessage.Send(new WhereToModel(PointA.Name, PointB.Name, PointA.Location, PointB.Location));

                NavigationService.GoBack();
            }
            else
            {
                if (PointA.HasValue() && PointB.HasValue())
                    NavigationService.NavigateTo(string.Format("/Views/WhereTo.xaml?startname={0}&endname={1}&startlat={2}&startlon={3}&endlat={4}&endlon={5}", PointA.Name, PointB.Name, PointA.Location.Latitude.ToString(CultureInfo.InvariantCulture), PointA.Location.Longitude.ToString(CultureInfo.InvariantCulture), PointB.Location.Latitude.ToString(CultureInfo.InvariantCulture), PointB.Location.Longitude.ToString(CultureInfo.InvariantCulture)));
                else if (PointA.HasValue() && !PointB.HasValue())
                    NavigationService.NavigateTo(string.Format("/Views/WhereTo.xaml?startname={0}&startlat={1}&startlon={2}", PointA.Name, PointA.Location.Latitude.ToString(CultureInfo.InvariantCulture), PointA.Location.Longitude.ToString(CultureInfo.InvariantCulture)));
                else if (!PointA.HasValue() && PointB.HasValue())
                    NavigationService.NavigateTo(string.Format("/Views/WhereTo.xaml?endname={0}&endlat={1}&endlon={2}", PointB.Name, PointB.Location.Latitude.ToString(CultureInfo.InvariantCulture), PointB.Location.Longitude.ToString(CultureInfo.InvariantCulture)));
            }
        }

        #endregion

        #region Timer Functions

        private void TimerTickMapMoved(object sender, EventArgs e)
        {
            this.timerMapMoved.Stop();

            MapStops.Clear();
            MapSearchResults.Clear();

            if (isStopSearch)
            {
                this.MapStops.AddRange(UnitOfWork.PublicStopRepository.GetNearby(numNearbyToLoadForMap, new Coordinate(MapCenterPoint.Latitude, MapCenterPoint.Longitude)));
            }
            else
            {
                this.MapSearchResults.AddRange(SearchResults);
            }
        }

        private void TimerTickSearch(object sender, EventArgs e)
        {
            timerSearch.Stop();

            ShowHeaderLoader();

            if (isStopSearch)
            {
                IEnumerable<PublicStop> stops = UnitOfWork.PublicStopRepository.GetByName(searchText);

                Stations.Clear();
                Stations.AddRange(stops);

                ShowStopsList();
                HideHeaderLoader();
            }
            else
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                Action search = async () =>
                {
                    try
                    {
                        SearchResults.Clear();
                        SearchResults.AddRange(await BumbleApiService.PlacesOfInterest(cancellationTokenSource.Token, user, SearchText, null));
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message != AppResources.ApiErrorTaskCancelled)
                        {
                            base.ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);
                        }

                        ClearSearchTextBox();
                    }

                    ShowSearchList();
                    HideHeaderLoader();
                };

                DispatcherHelper.CheckBeginInvokeOnUI(search);
            }
        }

        #endregion

        #region Message Received Functions

        private void SelectPointOnMap(PointOnMapMessage action)
        {
            switch (action.Reason)
            {
                case PointOnMapMessageReason.PublicStopPoint:
                    SelectedStation = UnitOfWork.PublicStopRepository.FindById(action.PublicStopPoint.PublicStopId);
                    break;
                case PointOnMapMessageReason.PlaceOfInterest:
                    SelectedSearchResult = action.PlaceOfInterest;
                    break;
                case PointOnMapMessageReason.CustomPoint:
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = new CancellationTokenSource();

                    ShowHeaderLoader();

                    Action search = async () =>
                    {
                        try
                        {
                            Address customAddress = await BumbleApiService.ReverseGeoCode(cancellationTokenSource.Token, user, action.Coordinate);

                            int distance = 0;

                            if (user.LastKnownGeneralLocation != null)
                                distance = (int)action.Coordinate.DistanceToCoordinateInMetres(user.LastKnownGeneralLocation);

                            SelectedCustomPoint = new Entities.PlaceOfInterest(customAddress.AddressText, null, customAddress.AddressText, action.Coordinate, distance);

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
                    break;
            }
        }

        private void SetSelectionMode(StationsAndPlacesOfInterestModeMessage mode)
        {
            ClearPoints();

            fromWhereTo = mode.FromWhereTo;

            switch (mode.Reason)
            {
                case Messages.Enums.StationsAndPlacesOfInterestModeReason.SearchPointA:
                    PointA.Select();

                    this.isPointA = true;

                    PointB.Deselect();

                    SwitchToList();

                    LoadSearch();
                    break;
                case Messages.Enums.StationsAndPlacesOfInterestModeReason.SearchPointB:
                    PointB.Select();

                    this.isPointA = false;

                    PointA.Deselect();

                    SwitchToList();

                    LoadSearch();
                    break;
                case Messages.Enums.StationsAndPlacesOfInterestModeReason.StationsPointA:
                    PointA.Select();

                    this.isPointA = true;

                    PointB.Deselect();

                    SwitchToList();

                    LoadStations();
                    break;
                case Messages.Enums.StationsAndPlacesOfInterestModeReason.StationsPointB:
                    PointB.Select();

                    this.isPointA = false;

                    PointA.Deselect();

                    SwitchToList();

                    LoadStations();
                    break;
                case Messages.Enums.StationsAndPlacesOfInterestModeReason.MapPointA:
                    PointA.Select();

                    this.isPointA = true;

                    PointB.Deselect();

                    SwitchToMap();

                    LoadStations();
                    break;
                case Messages.Enums.StationsAndPlacesOfInterestModeReason.MapPointB:
                    PointB.Select();

                    this.isPointA = false;

                    PointA.Deselect();

                    SwitchToMap();

                    LoadStations();
                    break;
            }
        }

        #endregion

        #region Local Static Functions

        private static void GoToSettings()
        {
            Messenger.Default.Send<Popup>(Popup.Settings);
        }

        private static void HideHoldMapPopup()
        {
            Messenger.Default.Send<Popup>(Popup.MapPointSelectionHint);
        }

        #endregion

        #region Overides

        protected override void UserLocationFound(GpsWatcherResponseMessage gpsWatcherResponseMessage)
        {
            base.UserLocationFound(gpsWatcherResponseMessage);

            if (gpsWatcherResponseMessage.Reason != Messages.Enums.GpsWatcherResponseMessageReason.Error)
            {
                if (SentCoordinateRequest || gpsWatcherResponseMessage.IsUsingLastKnown)
                {
                    MapCenterPoint = gpsWatcherResponseMessage.Coordinate.GeoCoordinate;
                    MapZoomLevel = 15;
                    UserLocation = gpsWatcherResponseMessage.Coordinate;
                    UserLocationVisibility = Visibility.Visible;
                }

                if (isStopSearch)
                {
                    AddStations();
                }
                else
                {
                    AddSearchResultsNearby();
                }
            }
            else if (SentCoordinateRequest || gpsWatcherResponseMessage.IsUsingLastKnown)
            {
                if (isStopSearch)
                {
                    AddStations();
                }
                else
                {
                    AddSearchResultsNearby();
                }
            }
        }

        protected override void PageLoaded()
        {
            pageLoaded = true;

            Messenger.Default.Register<PointOnMapMessage>(this, (action) => SelectPointOnMap(action));

            ClearSearchTextBox();

            ShowLoader();

            if (isPointA)
            {
                PageTitleMessage.Send(AppResources.HeaderPointA);
            }
            else
            {
                PageTitleMessage.Send(AppResources.HeaderPointB);
            }

            base.PageLoaded();

            Stations.Clear();
            SearchResults.Clear();

            if (InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value)
            {
                locationIsAllowed = true;

                if (user.IsLocationUptodate)
                {
                    UserLocationFound(new GpsWatcherResponseMessage(true, user.LastKnownGeneralLocation, GpsWatcherResponseMessageReason.Coordinate));
                }
                else
                {
                    RegisterWatcher();
                }
            }
            else
            {
                locationIsAllowed = false;

                if (isStopSearch)
                {
                    AddStations();
                }
                else
                {
                    AddSearchResultsNearby();
                }
            }

            pageLoaded = false;
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Messenger.Default.Unregister<PointOnMapMessage>(this);

            SearchResults.Clear();
            Stations.Clear();
            
            //PointA = null;
            //PointB = null;
        }

        #endregion

        #region Properties

        private Visibility noResultsVisibility = Visibility.Collapsed;
        public Visibility NoResultsVisibility
        {
            get { return noResultsVisibility; }
            set
            {
                noResultsVisibility = value;
                RaisePropertyChanged("NoResultsVisibility");
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

        private Coordinate userLocation;
        public Coordinate UserLocation
        {
            get { return userLocation; }
            set
            {
                userLocation = value;
                RaisePropertyChanged("UserLocation");
            }
        }

        private WhereToPoint pointA = new WhereToPoint(true);
        public WhereToPoint PointA
        {
            get { return pointA; }
            set
            {
                pointA = value;
                RaisePropertyChanged("PointA");
            }
        }

        private WhereToPoint pointB = new WhereToPoint(false);
        public WhereToPoint PointB
        {
            get { return pointB; }
            set
            {
                pointB = value;
                RaisePropertyChanged("PointB");
            }
        }

        // TODO: Config this:
        private GeoCoordinate mapCenterPoint = new GeoCoordinate(-33.9216, 18.4208);
        public GeoCoordinate MapCenterPoint
        {
            get { return mapCenterPoint; }
            set
            {
                mapCenterPoint = value;
                RaisePropertyChanged("MapCenterPoint");

                timerMapMoved.Stop();
                timerMapMoved.Start();
            }
        }

        private int mapZoomLevel = 15;
        public int MapZoomLevel
        {
            get { return mapZoomLevel; }
            set
            {
                mapZoomLevel = value;

                RaisePropertyChanged("MapZoomLevel");
            }
        }

        private Entities.PlaceOfInterest selectedCustomPoint;
        public Entities.PlaceOfInterest SelectedCustomPoint
        {
            get { return selectedCustomPoint; }
            set
            {
                selectedCustomPoint = value;
                RaisePropertyChanged("SelectedCustomPoint");

                if (selectedCustomPoint != null)
                {
                    if (isPointA)
                    {
                        PointA.Select();
                        PointB.Deselect();
                        PointA.SetAsCustomPoint(selectedCustomPoint);
                        PointA.Show();
                    }
                    else
                    {
                        PointB.Select();
                        PointA.Deselect();
                        PointB.SetAsCustomPoint(selectedCustomPoint);
                        PointB.Show();
                    }

                    MapCenterPoint = value.Location.GeoCoordinate;
                    MapZoomLevel = 15;

                    MapVisibility = Visibility.Visible;
                    ListVisibility = Visibility.Collapsed;

                    SelectedCustomPointForView = selectedCustomPoint;

                    SelectedSearchItemVisibility = Visibility.Collapsed;
                    SelectedCustomPointVisibility = Visibility.Visible;
                    SelectedStationVisibility = Visibility.Collapsed;

                    SelectedCustomPoint = null;
                }
            }
        }

        private Entities.PlaceOfInterest selectedSearchResult;
        public Entities.PlaceOfInterest SelectedSearchResult
        {
            get { return selectedSearchResult; }
            set
            {
                selectedSearchResult = value;
                RaisePropertyChanged("SelectedSearchResult");

                if (selectedSearchResult != null)
                {
                    if (isPointA)
                    {
                        PointA.Select();
                        PointB.Deselect();
                        PointA.SetAsPlaceOfInterest(selectedSearchResult);
                        PointA.Show();
                    }
                    else
                    {
                        PointB.Select();
                        PointA.Deselect();
                        PointB.SetAsPlaceOfInterest(selectedSearchResult);
                        PointB.Show();
                    }

                    MapCenterPoint = value.Location.GeoCoordinate;
                    MapZoomLevel = 15;

                    MapVisibility = Visibility.Visible;
                    ListVisibility = Visibility.Collapsed;

                    SelectedSearchItemForView = selectedSearchResult;

                    SelectedSearchItemVisibility = Visibility.Visible;
                    SelectedStationVisibility = Visibility.Collapsed;
                    SelectedCustomPointVisibility = Visibility.Collapsed;

                    SelectedSearchResult = null;
                }
            }
        }

        private PublicStop selectedStation;
        public PublicStop SelectedStation
        {
            get { return selectedStation; }
            set
            {
                selectedStation = value;
                RaisePropertyChanged("SelectedStation");

                if (selectedStation != null)
                {
                    if (isPointA)
                    {
                        PointA.Select();
                        PointB.Deselect();
                        PointA.SetAsStation(selectedStation);
                        PointA.Show();
                    }
                    else
                    {
                        PointB.Select();
                        PointA.Deselect();
                        PointB.SetAsStation(selectedStation);
                        PointB.Show();
                    }

                    MapCenterPoint = value.Location.GeoCoordinate;
                    MapZoomLevel = 15;

                    MapVisibility = Visibility.Visible;
                    ListVisibility = Visibility.Collapsed;

                    SelectedStationForView = selectedStation;

                    SelectedSearchItemVisibility = Visibility.Collapsed;
                    SelectedStationVisibility = Visibility.Visible;
                    SelectedCustomPointVisibility = Visibility.Collapsed;

                    SelectedStation = null;
                }
            }
        }

        private Entities.PlaceOfInterest selectedSearchItemForView;
        public Entities.PlaceOfInterest SelectedSearchItemForView
        {
            get { return selectedSearchItemForView; }
            set
            {
                selectedSearchItemForView = value;
                RaisePropertyChanged("SelectedSearchItemForView");
            }
        }

        private PublicStop selectedStationForView;
        public PublicStop SelectedStationForView
        {
            get { return selectedStationForView; }
            set
            {
                selectedStationForView = value;
                RaisePropertyChanged("SelectedStationForView");
            }
        }

        private Entities.PlaceOfInterest selectedCustomPointForView;
        public Entities.PlaceOfInterest SelectedCustomPointForView
        {
            get { return selectedCustomPointForView; }
            set
            {
                selectedCustomPointForView = value;
                RaisePropertyChanged("SelectedCustomPointForView");
            }
        }

        private Visibility clearSearchImageVisibility = Visibility.Collapsed;
        public Visibility ClearSearchImageVisibility
        {
            get { return clearSearchImageVisibility; }
            set
            {
                clearSearchImageVisibility = value;
                RaisePropertyChanged("ClearSearchImageVisibility");
            }
        }

        private Visibility switchToMapVisibility = Visibility.Collapsed;
        public Visibility SwitchToMapVisibility
        {
            get { return switchToMapVisibility; }
            set
            {
                switchToMapVisibility = value;
                RaisePropertyChanged("SwitchToMapVisibility");
            }
        }
        
        private Visibility searchImageVisibility = Visibility.Visible;
        public Visibility SearchImageVisibility
        {
            get { return searchImageVisibility; }
            set
            {
                searchImageVisibility = value;
                RaisePropertyChanged("SearchImageVisibility");
            }
        }

        private Visibility stopListVisibility = Visibility.Collapsed;
        public Visibility StopListVisibility
        {
            get { return stopListVisibility; }
            set
            {
                stopListVisibility = value;
                RaisePropertyChanged("StopListVisibility");
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

        private Visibility mapVisibility = Visibility.Collapsed;
        public Visibility MapVisibility
        {
            get { return mapVisibility; }
            set
            {
                mapVisibility = value;
                RaisePropertyChanged("MapVisibility");
            }
        }

        private Visibility searchListVisibility = Visibility.Collapsed;
        public Visibility SearchListVisibility
        {
            get { return searchListVisibility; }
            set
            {
                searchListVisibility = value;
                RaisePropertyChanged("SearchListVisibility");
            }
        }

        private Visibility loaderVisibility = Visibility.Visible;
        public Visibility LoaderVisibility
        {
            get { return loaderVisibility; }
            set
            {
                loaderVisibility = value;
                RaisePropertyChanged("LoaderVisibility");
            }
        }

        private Visibility searchVisibility = Visibility.Collapsed;
        public Visibility SearchVisibility
        {
            get { return searchVisibility; }
            set
            {
                searchVisibility = value;
                RaisePropertyChanged("SearchVisibility");
            }
        }

        private Visibility stationsVisibility = Visibility.Visible;
        public Visibility StationsVisibility
        {
            get { return stationsVisibility; }
            set
            {
                stationsVisibility = value;
                RaisePropertyChanged("StationsVisibility");
            }
        }

        private Visibility selectedStationVisibility = Visibility.Collapsed;
        public Visibility SelectedStationVisibility
        {
            get { return selectedStationVisibility; }
            set
            {
                selectedStationVisibility = value;
                RaisePropertyChanged("SelectedStationVisibility");
            }
        }

        private Visibility selectedCustomPointVisibility = Visibility.Collapsed;
        public Visibility SelectedCustomPointVisibility
        {
            get { return selectedCustomPointVisibility; }
            set
            {
                selectedCustomPointVisibility = value;
                RaisePropertyChanged("SelectedCustomPointVisibility");
            }
        }

        private Visibility selectedSearchItemVisibility = Visibility.Collapsed;
        public Visibility SelectedSearchItemVisibility
        {
            get { return selectedSearchItemVisibility; }
            set
            {
                selectedSearchItemVisibility = value;
                RaisePropertyChanged("SelectedSearchItemVisibility");
            }
        }
        
        private string searchText = AppResources.SearchStationsAndPlacesOfInterestTextBoxWatermark;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;

                if (timerSearch.IsEnabled)
                {
                    timerSearch.Stop();
                }

                if (searchText != null && searchText.Equals(AppResources.SearchStationsAndPlacesOfInterestTextBoxWatermark))
                {
                    ShowSearchIcon();

                    if (!pageLoaded && isStopSearch)
                        AddStations();
                    else if (!pageLoaded && !isStopSearch)
                        AddSearchResultsNearby();

                }
                else if (!String.IsNullOrEmpty(searchText))
                {
                    ShowDeleteIcon();

                    timerSearch.Start();
                }

                RaisePropertyChanged("SearchText");
            }
        }

        private ObservableCollection<PublicStop> stations = new ObservableCollection<PublicStop>();
        public ObservableCollection<PublicStop> Stations
        {
            get { return stations; }
        }

        private ObservableCollection<PublicStopPoint> mapStops = new ObservableCollection<PublicStopPoint>();
        public ObservableCollection<PublicStopPoint> MapStops
        {
            get { return mapStops; }
        }

        private ObservableCollection<Entities.PlaceOfInterest> searchResults = new ObservableCollection<Entities.PlaceOfInterest>();
        public ObservableCollection<Entities.PlaceOfInterest> SearchResults
        {
            get { return searchResults; }
        }

        private ObservableCollection<Entities.PlaceOfInterest> mapSearchResults = new ObservableCollection<Entities.PlaceOfInterest>();
        public ObservableCollection<Entities.PlaceOfInterest> MapSearchResults
        {
            get { return mapSearchResults; }
        }

        #endregion

        #region Commands

        public RelayCommand ClearSearchTextBoxCommand
        {
            get { return new RelayCommand(ClearSearch); }
        }

        public RelayCommand SearchTextBoxGotFocusCommand
        {
            get { return new RelayCommand(SearchTextBoxGotFocus); }
        }

        public RelayCommand SearchTextBoxLostFocusCommand
        {
            get { return new RelayCommand(SearchTextBoxLostFocus); }
        }

        public RelayCommand SearchCommand
        {
            get { return new RelayCommand(SwitchToSearch); }
        }

        public RelayCommand StationsCommand
        {
            get { return new RelayCommand(SwitchToStations); }
        }

        public RelayCommand SwitchToMapCommand
        {
            get { return new RelayCommand(SwitchToMap); }
        }

        public RelayCommand SwitchToListCommand
        {
            get { return new RelayCommand(SwitchToList); }
        }

        public RelayCommand ReCenterOnPointCommand
        {
            get { return new RelayCommand(ReCenterOnPoint); }
        }

        public RelayCommand PointACommand
        {
            get { return new RelayCommand(PointAClick); }
        }

        public RelayCommand PointBCommand
        {
            get { return new RelayCommand(PointBClick); }
        }

        public RelayCommand CenterUserLocationCommand
        {
            get { return new RelayCommand(CenterUserLocation); }
        }

        public RelayCommand RemovePointACommand
        {
            get { return new RelayCommand(this.RemovePointA); }
        }

        public RelayCommand RemovePointBCommand
        {
            get { return new RelayCommand(this.RemovePointB); }
        }

        public RelayCommand CancelCommand
        {
            get { return new RelayCommand(this.Cancel); }
        }

        public RelayCommand SubmitCommand
        {
            get { return new RelayCommand(this.Submit); }
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
