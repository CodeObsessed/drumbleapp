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

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class StopSelectionViewModel : AnalyticsBase, IDisposable
    {
        private DispatcherTimer timerSearch;
        private bool pageLoaded = false;

        public StopSelectionViewModel(IUnitOfWork unitOfWork, AppUse appUse, INavigationService navigationService)
            : base(appUse, ApplicationPage.StopSelection, unitOfWork, navigationService)
        {
            this.timerSearch = new DispatcherTimer();
            this.timerSearch.Interval = new TimeSpan(0, 0, 1);
            this.timerSearch.Tick += TimerTickSearch;
        }

        #region Local Functions

        private void TimerTickSearch(object sender, EventArgs e)
        {
            timerSearch.Stop();

            ShowHeaderLoader();

            IEnumerable<PublicStop> stops = unitOfWork.PublicStopRepository.GetByName(searchText);

            Stops.Clear();
            Stops.AddRange(stops);

            HideHeaderLoader();

            base.AddFeatureUse(PageUseType.TextBox, string.Format("Searched for [{0}]", searchText));
        }

        private void AddStops()
        {
            ShowLoader();
            Stops.Clear();
            Stops.AddRange(unitOfWork.PublicStopRepository.GetNearby(30));
            ShowList();
        }

        private void SearchTextBoxGotFocus()
        {
            if (SearchText.Equals(AppResources.SearchStationsTextBoxWaterMark))
            {
                SearchText = String.Empty;
            }
        }

        private void ClearSearchTextBox()
        {
            ShowSearchIcon();

            SearchText = AppResources.SearchStationsTextBoxWaterMark;
        }

        private void ClearSearch()
        {
            ClearSearchTextBox();

            base.AddFeatureUse(PageUseType.ImageButton, "Tapped the clear button");
        }

        private void SearchTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(SearchText))
            {
                SearchText = AppResources.SearchStationsTextBoxWaterMark;
            }
        }

        private void ShowList()
        {
            LoaderVisibility = Visibility.Collapsed;
            ListVisibility = Visibility.Visible;
        }

        private void ShowLoader()
        {
            LoaderVisibility = Visibility.Visible;
            ListVisibility = Visibility.Collapsed;
        }

        private void ShowHeaderLoader()
        {
            LoadingBarMessage.Send(LoadingBarMessageReason.Show);
        }

        private void HideHeaderLoader()
        {
            LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
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

        protected override void UserLocationFound(GpsWatcherResponseMessage gpsWatcherResponseMessage)
        {
            base.UserLocationFound(gpsWatcherResponseMessage);

            if (gpsWatcherResponseMessage.Reason != Messages.Enums.GpsWatcherResponseMessageReason.Error)
            {
                AddStops();
            }
            else if (SentCoordinateRequest || gpsWatcherResponseMessage.IsUsingLastKnown)
            {
                AddStops();
            }
        }

        protected override void PageLoaded()
        {
            pageLoaded = true;

            ClearSearchTextBox();

            ShowLoader();

            base.PageLoaded();

            Stops.Clear();

            if (unitOfWork.AppSettingRepository.FindByType(ApplicationSetting.AllowLocation).Value)
            {
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
                AddStops();
            }

            pageLoaded = false;
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Stops.Clear();
        }

        #endregion

        #region Properties

        private PublicStop selectedStop;
        public PublicStop SelectedStop
        {
            get { return selectedStop; }
            set
            {
                selectedStop = value;
                RaisePropertyChanged("SelectedStop");

                if (value != null)
                {
                    Messenger.Default.Send<PublicStop>(value);

                    SelectedStop = null;

                    base.AddFeatureUse(PageUseType.List, string.Format("Selected stop [{0}]", value.Name));

                    navigationService.GoBack();
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

        private string searchText = AppResources.SearchStationsTextBoxWaterMark;
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

                if (searchText != null && searchText.Equals(AppResources.SearchStationsTextBoxWaterMark))
                {
                    ShowSearchIcon();

                    if (!pageLoaded)
                        AddStops();
                }
                else if (!String.IsNullOrEmpty(searchText))
                {
                    ShowDeleteIcon();

                    timerSearch.Start();
                }

                RaisePropertyChanged("SearchText");
            }
        }

        private ObservableCollection<PublicStop> stops = new ObservableCollection<PublicStop>();
        public ObservableCollection<PublicStop> Stops
        {
            get { return stops; }
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
