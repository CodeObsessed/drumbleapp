using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Threading;
using DrumbleApp.Shared.Resources;
using System.Windows;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using System.Linq;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class FavouritesViewModel : AnalyticsBase, IDisposable
    {
        private DispatcherTimer timerSearch;
        private bool pageLoaded = false;
        private bool fromWhereTo = false;
        private bool isFavouritesView = true;

        public FavouritesViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.Favourites, aggregateService)
        {
            this.timerSearch = new DispatcherTimer();
            this.timerSearch.Interval = new TimeSpan(0, 0, 1);
            this.timerSearch.Tick += TimerTickSearch;

            Messenger.Default.Register<FavouriteMessage>(this, (action) => SetFavourite(action));
            Messenger.Default.Register<RecentTripMessage>(this, (action) => SetRecentTrip(action));

            Messenger.Default.Register<FavouritesPageState>(this, (action) => PageState(action));
        }

        #region Local Functions

        private void ShowFavourites()
        {
            isFavouritesView = true;

            ClearSearchTextBox();
            RecentTrips.Clear();

            FavouritesVisibility = Visibility.Visible;
            RecentVisibility = Visibility.Collapsed;

            PageTitleMessage.Send(AppResources.HeaderFavourites);

            AddFavourites();
        }

        private void ShowRecent()
        {
            isFavouritesView = false;

            ClearSearchTextBox();
            Favourites.Clear();

            FavouritesVisibility = Visibility.Collapsed;
            RecentVisibility = Visibility.Visible;

            PageTitleMessage.Send(AppResources.HeaderRecent);

            AddRecentTrips();
        }

        private void PageState(FavouritesPageState action)
        {
            switch (action)
            {
                case FavouritesPageState.FromWhereTo:
                    fromWhereTo = true;
                    break;
                case FavouritesPageState.Favourites:
                    if (!isFavouritesView)
                    {
                        ShowFavourites();
                    }
                    break;
                case FavouritesPageState.Recent:
                    if (isFavouritesView)
                    {
                        ShowRecent();
                    }
                    break;
            }
        }

        private void SetFavourite(FavouriteMessage favouriteMessage)
        {
            switch (favouriteMessage.Reason)
            {
                case Messages.Enums.FavouriteMessageReason.RemoveFromFavourites:
                    UnitOfWork.FavouriteRepository.Delete(favouriteMessage.Favourite);
                    UnitOfWork.Save();

                    ClearSearchTextBox();

                    base.ShowPopup(CustomPopupMessageType.Sucess, AppResources.FavouritesRemovedFromFavourites, AppResources.CustomPopupGenericOkMessage, null);
                    break;
            }
        }

        private void SetRecentTrip(RecentTripMessage recentTripMessage)
        {
            switch (recentTripMessage.Reason)
            {
                case Messages.Enums.RecentTripMessageReason.AddToFavourites:
                    UnitOfWork.RecentTripRepository.Update(recentTripMessage.RecentTrip);
                    UnitOfWork.Save();
                    base.ShowPopup(CustomPopupMessageType.Sucess, AppResources.RecentTripsAddedToFavourites, AppResources.CustomPopupGenericOkMessage, null);
                    break;
                case Messages.Enums.RecentTripMessageReason.RemoveFromFavourites:
                    UnitOfWork.RecentTripRepository.Update(recentTripMessage.RecentTrip);
                    UnitOfWork.Save();
                    base.ShowPopup(CustomPopupMessageType.Sucess, AppResources.RecentTripsRemovedFromFavourites, AppResources.CustomPopupGenericOkMessage, null);
                    break;
            }
        }

        private void TimerTickSearch(object sender, EventArgs e)
        {
            timerSearch.Stop();

            ShowHeaderLoader();

            if (isFavouritesView)
            {
                IEnumerable<Favourite> favourites = UnitOfWork.FavouriteRepository.GetByName(searchText);

                Favourites.Clear();
                Favourites.AddRange(favourites);
            }
            else
            {
                IEnumerable<Recent> recentTrips = UnitOfWork.RecentTripRepository.GetByName(searchText);

                RecentTrips.Clear();
                RecentTrips.AddRange(recentTrips);
            }

            HideHeaderLoader();

            ShowList();
        }

        private void AddFavourites()
        {
            ShowLoader();
            Favourites.Clear();
            Favourites.AddRange(UnitOfWork.FavouriteRepository.GetAll().OrderByDescending(x => x.LastUsedDate));
            ShowList();
        }

        private void AddRecentTrips()
        {
            ShowLoader();
            RecentTrips.Clear();
            RecentTrips.AddRange(UnitOfWork.RecentTripRepository.GetAll().OrderByDescending(x => x.LastUsedDate));
            ShowList();
        }

        private void SearchTextBoxGotFocus()
        {
            if (SearchText.Equals(AppResources.FavouritesSearchTextBoxWaterMark))
            {
                SearchText = String.Empty;
            }
        }

        private void ClearSearchTextBox()
        {
            ShowSearchIcon();

            SearchText = AppResources.FavouritesSearchTextBoxWaterMark;
        }

        private void ClearSearch()
        {
            ClearSearchTextBox();
        }

        private void SearchTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                SearchText = AppResources.FavouritesSearchTextBoxWaterMark;
            }
        }

        private void ShowList()
        {
            LoaderVisibility = Visibility.Collapsed;

            if (isFavouritesView)
            {
                if (Favourites.Count == 0)
                {
                    NoResultsVisibility = Visibility.Visible;
                    FavouritesListVisibility = Visibility.Collapsed;
                }
                else
                {
                    FavouritesListVisibility = Visibility.Visible;
                    NoResultsVisibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (RecentTrips.Count == 0)
                {
                    NoResultsVisibility = Visibility.Visible;
                    RecentTripsListVisibility = Visibility.Collapsed;
                }
                else
                {
                    RecentTripsListVisibility = Visibility.Visible;
                    NoResultsVisibility = Visibility.Collapsed;
                }
            }
        }

        private void ShowLoader()
        {
            LoaderVisibility = Visibility.Visible;
            RecentTripsListVisibility = Visibility.Collapsed;
            FavouritesListVisibility = Visibility.Collapsed;
            NoResultsVisibility = Visibility.Collapsed;
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

        #endregion

        #region Overides

        protected override void PageLoaded()
        {
            

            pageLoaded = true;

            ClearSearchTextBox();

            ShowLoader();

            base.PageLoaded();

            if (isFavouritesView)
            {
                ShowFavourites();
            }
            else
            {
                ShowRecent();
            }

            pageLoaded = false;
        }

        #endregion

        #region Properties

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

                    if (fromWhereTo)
                    {
                        RecentTripMessage.Send(value, Messages.Enums.RecentTripMessageReason.SetAsWhereToDestination);

                        NavigationService.GoBack();
                    }
                    else
                    {
                        NavigationService.NavigateTo(string.Format("/Views/WhereTo.xaml?recentTripId={0}", value.Id.ToString()));
                    }
                }
            }
        }

        private Favourite selectedFavourite;
        public Favourite SelectedFavourite
        {
            get { return selectedFavourite; }
            set
            {
                selectedFavourite = value;
                RaisePropertyChanged("SelectedFavourite");

                if (value != null)
                {
                    SelectedFavourite = null;

                    if (fromWhereTo)
                    {
                        FavouriteMessage.Send(value, Messages.Enums.FavouriteMessageReason.SetAsWhereTo);

                        NavigationService.GoBack();
                    }
                    else
                    {
                        NavigationService.NavigateTo(string.Format("/Views/WhereTo.xaml?favouriteId={0}", value.Id.ToString()));
                    }
                }
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

        private Visibility favouritesListVisibility = Visibility.Collapsed;
        public Visibility FavouritesListVisibility
        {
            get { return favouritesListVisibility; }
            set
            {
                favouritesListVisibility = value;
                RaisePropertyChanged("FavouritesListVisibility");
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

        private Visibility favouritesVisibility = Visibility.Visible;
        public Visibility FavouritesVisibility
        {
            get { return favouritesVisibility; }
            set
            {
                favouritesVisibility = value;
                RaisePropertyChanged("FavouritesVisibility");
            }
        }

        private Visibility recentVisibility = Visibility.Collapsed;
        public Visibility RecentVisibility
        {
            get { return recentVisibility; }
            set
            {
                recentVisibility = value;
                RaisePropertyChanged("RecentVisibility");
            }
        }

        private string searchText = AppResources.FavouritesSearchTextBoxWaterMark;
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

                if (searchText != null && searchText.Equals(AppResources.FavouritesSearchTextBoxWaterMark))
                {
                    ShowSearchIcon();

                    if (!pageLoaded)
                    {
                        if (isFavouritesView)
                            AddFavourites();
                        else
                            AddRecentTrips();
                    }
                }
                else if (!String.IsNullOrEmpty(searchText))
                {
                    ShowDeleteIcon();

                    timerSearch.Start();
                }

                RaisePropertyChanged("SearchText");
            }
        }

        private ObservableCollection<Favourite> favourites = new ObservableCollection<Favourite>();
        public ObservableCollection<Favourite> Favourites
        {
            get { return favourites; }
        }

        private ObservableCollection<Recent> recentTrips = new ObservableCollection<Recent>();
        public ObservableCollection<Recent> RecentTrips
        {
            get { return recentTrips; }
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

        public RelayCommand FavouritesCommand
        {
            get { return new RelayCommand(ShowFavourites); }
        }

        public RelayCommand RecentCommand
        {
            get { return new RelayCommand(ShowRecent); }
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
