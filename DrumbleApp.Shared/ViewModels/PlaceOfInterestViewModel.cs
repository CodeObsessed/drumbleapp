using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using DrumbleApp.Shared.Infrastructure.Extensions;
using GalaSoft.MvvmLight.Command;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using System.Threading;
using System.Linq;
using GalaSoft.MvvmLight.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class PlaceOfInterestViewModel : AnalyticsBase, IDisposable
    {
        private IBumbleApiService BumbleApiService;
        private DispatcherTimer timerSearch;
        private bool pageLoaded = false;
        private bool locationIsAllowed = false;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private IEnumerable<PlaceOfInterestCategory> poiCategories;

        public PlaceOfInterestViewModel(IUnitOfWork unitOfWork, IBumbleApiService BumbleApiService, AppUse appUse, INavigationService navigationService)
            : base(appUse, ApplicationPage.ExploreCity, unitOfWork, navigationService)
        {
            this.timerSearch = new DispatcherTimer();
            this.timerSearch.Interval = new TimeSpan(0, 0, 1);
            this.timerSearch.Tick += TimerTickSearch;

            this.BumbleApiService = BumbleApiService;
        }

        #region Local Functions

        private void FilterButton()
        {
            navigationService.NavigateTo("/Views/FilterPlaceOfInterestCategories.xaml");
        }
        
        private void TimerTickSearch(object sender, EventArgs e)
        {
            timerSearch.Stop();
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            HideNoResults();
            ShowLoader();

            Action search = async () =>
            {
                try
                {
                    PlacesOfInterest.Clear();

                    IEnumerable<Entities.PlaceOfInterest> results;
                    
                    if (locationIsAllowed && user.LastKnownGeneralLocation != null && user.LastKnownGeneralLocation.IsValid())
                        results = await BumbleApiService.PlacesOfInterest(cancellationTokenSource.Token, user, SearchText, null);
                    else
                        results = await BumbleApiService.PlacesOfInterest(cancellationTokenSource.Token, user, SearchText, null);

                    PlacesOfInterest.AddRange(results.Where(x => poiCategories.Where(z => z.IsChecked).Select(y => y.Category).Contains(x.PlaceOfInterestCategory.Category)));

                    HideLoader();
                    if (PlacesOfInterest.Count() == 0)
                        ShowNoResults();

                    base.AddFeatureUse(PageUseType.TextBox, string.Format("Searched for [{0}]", searchText));
                }
                catch (Exception ex)
                {
                    if (ex.Message != AppResources.ApiErrorTaskCancelled)
                    {
                        base.ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);

                        base.AddFeatureUse(PageUseType.Error, string.Format("Error while searching. Reason: {0}", ex.Message));

                        HideLoader();
                        ShowNoResults();
                        ClearSearchTextBox();
                    }                   
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(search);
        }

        private void SearchTextBoxGotFocus()
        {
            if (SearchText.Equals(AppResources.PlacesOfInterestTextBoxWaterMark))
            {
                SearchText = String.Empty;
            }
        }

        private void SearchTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                SearchText = AppResources.PlacesOfInterestTextBoxWaterMark;
            }
        }

        private void ShowItemSelected()
        {
            HideLoader();
            HideNoResults();
            ListVisibility = Visibility.Collapsed;
            ItemSelectedFooterVisibility = Visibility.Visible;
            MapFooterVisibility = Visibility.Collapsed;
            SearchBoxVisibility = Visibility.Collapsed;
        }

        private void ShowMap()
        {
            HideLoader();
            HideNoResults();
            ListVisibility = Visibility.Visible;
            ItemSelectedFooterVisibility = Visibility.Collapsed;
            MapFooterVisibility = Visibility.Visible;
            SearchBoxVisibility = Visibility.Visible;
        }

        private void ShowList()
        {
            HideLoader();
            HideNoResults();
            ListVisibility = Visibility.Visible;
            ItemSelectedFooterVisibility = Visibility.Collapsed;
            SearchBoxVisibility = Visibility.Visible;
            MapFooterVisibility = Visibility.Collapsed;
        }

        private void ShowLoader()
        {
            LoaderIsIndeterminate = true;
            LoaderVisibility = Visibility.Visible;
            NoResultsVisibility = Visibility.Collapsed;
        }

        private void HideLoader()
        {
            LoaderIsIndeterminate = false;
            LoaderVisibility = Visibility.Collapsed;
        }

        private void ShowNoResults()
        {
            NoResultsVisibility = Visibility.Visible;
        }

        private void HideNoResults()
        {
            NoResultsVisibility = Visibility.Collapsed;
        }

        private void ShowHeaderLoader()
        {
            LoadingBarMessage.Send(LoadingBarMessageReason.Show);
        }

        private void HideHeaderLoader()
        {
            LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
        }

        private void ClearSearchTextBox()
        {
            SearchText = AppResources.PlacesOfInterestTextBoxWaterMark;
        }

        private void ListCancelButton()
        {
            navigationService.GoBack();
        }

        private void MapButton()
        {
            if (PlacesOfInterest.Count() == 0)
            {
                base.ShowPopup(CustomPopupMessageType.Information, AppResources.PlacesOfInterestNoMapResultsPopupText, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            ShowMap();
        }

        private void ListButton()
        {
            ShowList();
        }
        
        
        #endregion

        #region Overides

        protected override void PageLoaded()
        {
            pageLoaded = true;

            ClearSearchTextBox();

            ShowNoResults();

            base.PageLoaded();

            if (unitOfWork.AppSettingRepository.FindByType(ApplicationSetting.AllowLocation).Value)
            {
                locationIsAllowed = true;

                if (user.IsLocationUptodate)
                {
                    UserLocationFound(new GpsWatcherResponseMessage(true, user.LastKnownGeneralLocation, GpsWatcherResponseMessageReason.Coordinate));
                }
                else
                {
                    base.RegisterWatcher();
                }
            }
            else
            {
                locationIsAllowed = false;
            }

            poiCategories = unitOfWork.PlaceOfInterestCategoryRepository.GetAll();

            if (poiCategories.Any(x => !x.IsChecked))
            {
                FilterOnVisibility = Visibility.Visible;
                FilterOffVisibility = Visibility.Collapsed;
            }
            else
            {
                FilterOnVisibility = Visibility.Collapsed;
                FilterOffVisibility = Visibility.Visible;
            }

            pageLoaded = false;
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            PlacesOfInterest.Clear();
            poiCategories = null;
        }

        protected override void UserLocationFound(GpsWatcherResponseMessage gpsWatcherResponseMessage)
        {
            base.UserLocationFound(gpsWatcherResponseMessage);

            MapCenterPoint = gpsWatcherResponseMessage.Coordinate;
        }

        #endregion

        #region Properties

        private Entities.PlaceOfInterest selectedPlaceOfInterest;
        public Entities.PlaceOfInterest SelectedPlaceOfInterest
        {
            get { return selectedPlaceOfInterest; }
            set
            {
                selectedPlaceOfInterest = value;

                if (value != null)
                {
                    RaisePropertyChanged("SelectedPlaceOfInterest");

                    SelectedPlaceOfInterest = null;

                    ShowItemSelected();

                    MapCenterPoint = value.Location;
                    MapZoomLevel = 15;
                    
                    base.AddFeatureUse(PageUseType.List, string.Format("Selected POI [{0}]", value.Name));
                }
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

        private Visibility loaderVisibility = Visibility.Collapsed;
        public Visibility LoaderVisibility
        {
            get { return loaderVisibility; }
            set
            {
                loaderVisibility = value;
                RaisePropertyChanged("LoaderVisibility");
            }
        }

        private Visibility searchBoxVisibility = Visibility.Visible;
        public Visibility SearchBoxVisibility
        {
            get { return searchBoxVisibility; }
            set
            {
                searchBoxVisibility = value;
                RaisePropertyChanged("SearchBoxVisibility");
            }
        }

        private bool loaderIsIndeterminate = false;
        public bool LoaderIsIndeterminate
        {
            get { return loaderIsIndeterminate; }
            set
            {
                loaderIsIndeterminate = value;
                RaisePropertyChanged("LoaderIsIndeterminate");
            }
        }

        private Visibility mapFooterVisibility = Visibility.Collapsed;
        public Visibility MapFooterVisibility
        {
            get { return mapFooterVisibility; }
            set
            {
                mapFooterVisibility = value;
                RaisePropertyChanged("MapFooterVisibility");
            }
        }
       
        private Visibility itemSelectedFooterVisibility = Visibility.Collapsed;
        public Visibility ItemSelectedFooterVisibility
        {
            get { return itemSelectedFooterVisibility; }
            set
            {
                itemSelectedFooterVisibility = value;
                RaisePropertyChanged("ItemSelectedFooterVisibility");
            }
        }

        private Visibility filterOffVisibility = Visibility.Visible;
        public Visibility FilterOffVisibility
        {
            get { return filterOffVisibility; }
            set
            {
                filterOffVisibility = value;
                RaisePropertyChanged("FilterOffVisibility");
            }
        }

        private Visibility filterOnVisibility = Visibility.Collapsed;
        public Visibility FilterOnVisibility
        {
            get { return filterOnVisibility; }
            set
            {
                filterOnVisibility = value;
                RaisePropertyChanged("FilterOnVisibility");
            }
        }
        
        private string searchText = AppResources.PlacesOfInterestTextBoxWaterMark;
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

                if (searchText != null && searchText.Equals(AppResources.PlacesOfInterestTextBoxWaterMark))
                {
                    if (!pageLoaded)
                    {
                        if (PlacesOfInterest.Count() == 0)
                            ShowNoResults();

                        HideLoader();
                    }

                }
                else if (!String.IsNullOrEmpty(searchText))
                {
                    timerSearch.Start();
                }

                RaisePropertyChanged("SearchText");
            }
        }

        private ObservableCollection<Entities.PlaceOfInterest> placesOfInterest = new ObservableCollection<Entities.PlaceOfInterest>();
        public ObservableCollection<Entities.PlaceOfInterest> PlacesOfInterest
        {
            get { return placesOfInterest; }
        }

        // TODO: Config this:
        private Coordinate mapCenterPoint = new Coordinate(-33.9216, 18.4208);
        public Coordinate MapCenterPoint
        {
            get { return mapCenterPoint; }
            set
            {
                mapCenterPoint = value;

                RaisePropertyChanged("MapCenterPoint");
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

        public RelayCommand SearchTextBoxGotFocusCommand
        {
            get { return new RelayCommand(SearchTextBoxGotFocus); }
        }

        public RelayCommand SearchTextBoxLostFocusCommand
        {
            get { return new RelayCommand(SearchTextBoxLostFocus); }
        }

        public RelayCommand FilterButtonCommand
        {
            get { return new RelayCommand(FilterButton); }
        }

        public RelayCommand ListCancelButtonCommand
        {
            get { return new RelayCommand(ListCancelButton); }
        }

        public RelayCommand MapButtonCommand
        {
            get { return new RelayCommand(MapButton); }
        }

        public RelayCommand ListButtonCommand
        {
            get { return new RelayCommand(ListButton); }
        }

        public RelayCommand MapCancelButtonCommand
        {
            get { return new RelayCommand(ListButton); }
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
                    unitOfWork.Dispose();
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
