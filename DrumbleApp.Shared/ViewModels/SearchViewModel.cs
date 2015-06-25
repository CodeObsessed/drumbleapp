using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using DrumbleApp.Shared.Infrastructure.Extensions;
using GalaSoft.MvvmLight.Command;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using GalaSoft.MvvmLight.Ioc;
using DrumbleApp.Shared.Models;
using System.Collections.Generic;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class SearchViewModel : AnalyticsBase, IDisposable
    {
        private IBumbleApiService BumbleApiService;
        private DispatcherTimer timerSearch;
        private SearchType searchType;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SearchViewModel(IAggregateService aggregateService, IBumbleApiService BumbleApiService)
            : base(ApplicationPage.Search, aggregateService)
        {
            this.timerSearch = new DispatcherTimer();
            this.timerSearch.Interval = new TimeSpan(0, 0, 1);
            this.timerSearch.Tick += TimerTickSearch;

            this.BumbleApiService = BumbleApiService;
        }

        #region Local Functions

        private void ViewSearchItemOnMap(SearchItemMessage searchItem)
        {
            if (searchItem.Reason == SearchItemMessageReason.ViewOnMap)
            {
                if (SimpleIoc.Default.IsRegistered<SearchTypeModel>())
                    SimpleIoc.Default.Unregister<SearchTypeModel>();

                SimpleIoc.Default.Register<SearchTypeModel>(() =>
                {
                    return new SearchTypeModel(searchType);
                });

                if (SimpleIoc.Default.IsRegistered<SearchItem>())
                    SimpleIoc.Default.Unregister<SearchItem>();

                SimpleIoc.Default.Register<SearchItem>(() =>
                {
                    return searchItem.SearchItem;
                });

                NavigationService.NavigateTo("/Views/MapPointSelection.xaml");
            }
        }

        private void TimerTickSearch(object sender, EventArgs e)
        {
            timerSearch.Stop();
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            // No longer on this page.
            if (user == null)
                return;

            ShowLoader();
            
            Action search = async () =>
            {
                try
                {
                    IEnumerable<SearchItem> results = null;
                    if (base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value && user.LastKnownGeneralLocation != null && user.LastKnownGeneralLocation.IsValid())
                        results = await BumbleApiService.Search(cancellationTokenSource.Token, user, SearchText, user.LastKnownGeneralLocation);
                    else
                        results = await BumbleApiService.Search(cancellationTokenSource.Token, user, SearchText, null);

                    SearchResults.Clear();
                    if (results != null)
                        searchResults.AddRange(results);
                    
                    ShowSearchList();
                }
                catch (Exception ex)
                {
                    if (ex.Message != AppResources.ApiErrorTaskCancelled)
                    {
                        ShowSearchList();
                        ClearSearchTextBox(true);

                        base.ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);
                    }
                }

                HideLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(search);
        }

        private void SearchTextBoxGotFocus()
        {
            if (SearchText.Equals(AppResources.SearchTextBoxWaterMark))
            {
                SearchText = String.Empty;
            }
        }

        private void ClearSearchTextBox(bool showWatermark)
        {
            cancellationTokenSource.Cancel();

            ShowSearchIcon();

            if (showWatermark)
                SearchText = AppResources.SearchTextBoxWaterMark;
            else
                SearchText = String.Empty;
        }

        private void ClearSearch()
        {
            ClearSearchTextBox(false);

            ShowRecentTrips();
        }

        private void SearchTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                SearchText = AppResources.SearchTextBoxWaterMark;
            }
        }

        private void ShowRecentTrips()
        {
            HideSearchList();

            RecentTrips.Clear();
            RecentTrips.AddRange(UnitOfWork.RecentTripRepository.GetAll());

            RecentRowVisibility = Visibility.Visible;

            if (this.RecentTrips.Count == 0)
            {
                RecentTripsListVisibility = Visibility.Collapsed;
                NoRecentsVisibility = Visibility.Visible;
            }
            else
            {
                RecentTripsListVisibility = Visibility.Visible;
                NoRecentsVisibility = Visibility.Collapsed;
            }
        }

        private void HideRecentTrips()
        {
            RecentRowVisibility = Visibility.Collapsed;
            RecentTripsListVisibility = Visibility.Collapsed;
            NoRecentsVisibility = Visibility.Collapsed;
        }

        private void ShowSearchList()
        {
            HideRecentTrips();

            SearchResultRowVisibility = Visibility.Visible;

            if (this.SearchResults.Count == 0)
            {
                ListVisibility = Visibility.Collapsed;
                NoSearchResultsVisibility = Visibility.Visible;
            }
            else
            {
                ListVisibility = Visibility.Visible;
                NoSearchResultsVisibility = Visibility.Collapsed;
            }
        }

        private void HideSearchList()
        {
            SearchResultRowVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Collapsed;
            NoSearchResultsVisibility = Visibility.Collapsed;
        }
        
        private void ShowLoader()
        {
            HideRecentTrips();

            SearchResultRowVisibility = Visibility.Visible;

            LoaderIsIndeterminate = true;
            LoaderVisibility = Visibility.Visible;
        }

        private void HideLoader()
        {
            LoaderIsIndeterminate = false;
            LoaderVisibility = Visibility.Collapsed;
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

        private void CurrentLocation()
        {
            if (!base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value)
            {
                ShowPopup(CustomPopupMessageType.Error, AppResources.LocationAppDisabledErrorMessage, AppResources.CustomPopupGenericOkGotItMessage, null);
                return;
            }

            WhereToMessage.Send(new Models.WhereToModel(string.Empty, null, searchType));

            NavigationService.GoBack();
        }

        private void FindOnMap()
        {
            if (SimpleIoc.Default.IsRegistered<SearchTypeModel>())
                SimpleIoc.Default.Unregister<SearchTypeModel>();

            SimpleIoc.Default.Register<SearchTypeModel>(() =>
            {
                return new SearchTypeModel(searchType);
            });

            NavigationService.NavigateTo("/Views/MapPointSelection.xaml");
        }

        #endregion

        #region Overides

        protected override void PageLoaded()
        {
            Messenger.Default.Register<SearchItemMessage>(this, (action) => ViewSearchItemOnMap(action));

            if (SimpleIoc.Default.IsRegistered<SearchTypeModel>())
            {
                searchType = SimpleIoc.Default.GetInstance<SearchTypeModel>().SearchType;

                SimpleIoc.Default.Unregister<SearchTypeModel>();

                if (searchType == SearchType.Location)
                    PageTitleMessage.Send(AppResources.HeaderChooseLocation);
                else
                    PageTitleMessage.Send(AppResources.HeaderChooseDestination);

                SearchText = AppResources.SearchTextBoxWaterMark;

                ShowRecentTrips();
            }
            else
            {
                if (SearchText == AppResources.SearchTextBoxWaterMark)
                    ShowRecentTrips();
            }

            base.PageLoaded();
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Messenger.Default.Unregister<SearchItemMessage>(this);

            //SearchResults.Clear();
        }

        #endregion

        #region Properties

        private SearchItem selectedSearchResult;
        public SearchItem SelectedSearchResult
        {
            get { return selectedSearchResult; }
            set
            {
                selectedSearchResult = value;
                RaisePropertyChanged("SelectedSearchResult");

                if (value != null)
                {
                    WhereToMessage.Send(new Models.WhereToModel(value.Name, value.Location, searchType));

                    SelectedSearchResult = null;

                    NavigationService.GoBack();
                }
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


        private Visibility noSearchResultsVisibility = Visibility.Collapsed;
        public Visibility NoSearchResultsVisibility
        {
            get { return noSearchResultsVisibility; }
            set
            {
                noSearchResultsVisibility = value;
                RaisePropertyChanged("NoSearchResultsVisibility");
            }
        }

        private Visibility searchResultRowVisibility = Visibility.Collapsed;
        public Visibility SearchResultRowVisibility
        {
            get { return searchResultRowVisibility; }
            set
            {
                searchResultRowVisibility = value;
                RaisePropertyChanged("SearchResultRowVisibility");
            }
        }

        private Visibility recentRowVisibility = Visibility.Visible;
        public Visibility RecentRowVisibility
        {
            get { return recentRowVisibility; }
            set
            {
                recentRowVisibility = value;
                RaisePropertyChanged("RecentRowVisibility");
            }
        }
        
        private Visibility noRecentsVisibility = Visibility.Visible;
        public Visibility NoRecentsVisibility
        {
            get { return noRecentsVisibility; }
            set
            {
                noRecentsVisibility = value;
                RaisePropertyChanged("NoRecentsVisibility");
            }
        }

        private Visibility recentTripsListVisibility = Visibility.Collapsed;
        public Visibility RecentTripsListVisibility
        {
            get { return recentTripsListVisibility; }
            set
            {
                recentTripsListVisibility = value;
                RaisePropertyChanged("RecentTripsListVisibility");
            }
        }

        private Visibility listVisibility = Visibility.Collapsed;
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

        private string searchText = AppResources.SearchTextBoxWaterMark;
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

                if (searchText != null && searchText.Equals(AppResources.SearchTextBoxWaterMark))
                {
                    ShowSearchIcon();
                }
                else if (!String.IsNullOrEmpty(searchText))
                {
                    ShowDeleteIcon();

                    timerSearch.Start();
                }

                RaisePropertyChanged("SearchText");
            }
        }

        private Recent selectedRecentTrip;
        public Recent SelectedRecentTrip
        {
            get { return selectedRecentTrip; }
            set
            {
                selectedRecentTrip = value;
                RaisePropertyChanged("SelectedRecentTrip");

                if (value != null)
                {
                    SelectedRecentTrip = null;

                    if (searchType == SearchType.Location)
                        RecentTripMessage.Send(value, Messages.Enums.RecentTripMessageReason.SetAsWhereToLocation);
                    else if (searchType == SearchType.Destination)
                        RecentTripMessage.Send(value, Messages.Enums.RecentTripMessageReason.SetAsWhereToDestination);

                    NavigationService.GoBack();
                }
            }
        }

        private ObservableCollection<SearchItem> searchResults = new ObservableCollection<SearchItem>();
        public ObservableCollection<SearchItem> SearchResults
        {
            get
            {
                return searchResults;
            }
        }

        private ObservableCollection<Recent> recentTrips = new ObservableCollection<Recent>();
        public ObservableCollection<Recent> RecentTrips
        {
            get { return recentTrips; }
        }

        #endregion

        #region Commands

        public RelayCommand FindOnMapCommand
        {
            get { return new RelayCommand(FindOnMap); }
        }

        public RelayCommand CurrentLocationCommand
        {
            get { return new RelayCommand(CurrentLocation); }
        }

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
