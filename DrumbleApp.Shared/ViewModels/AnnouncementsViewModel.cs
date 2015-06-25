using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DrumbleApp.Shared.Infrastructure.Extensions;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Shell;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class AnnouncementsViewModel : AnalyticsBase, IDisposable
    {
        private IBumbleApiService BumbleApi;
        private bool isLoadingAnnouncements = false;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public AnnouncementsViewModel(IAggregateService aggregateService, IBumbleApiService BumbleApi)
            : base(ApplicationPage.Announcements, aggregateService)
        {
            this.BumbleApi = BumbleApi;
        }

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderAnnouncements);

            LoadAnnouncements();

            if (base.InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowAnnouncementsApplicationBar).Value)
                ApplicationBarIsVisibile = true;
            else
                ApplicationBarIsVisibile = false;

            Messenger.Default.Register<ShakeGestureMessage>(this, (action) => ShakeGesture(action));
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Messenger.Default.Unregister<ShakeGestureMessage>(this);
        }

        #endregion

        #region Local Functions

        private void PinAnnouncements()
        {
            /*string periodicTaskName = "AnnouncementPeriodicTask";
            // is old task running, remove it
            PeriodicTask periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                try
                {
                    ScheduledActionService.Remove(periodicTaskName);
                }
                catch (Exception)
                {
                }
            }
            // create a new task
            periodicTask = new PeriodicTask(periodicTaskName);
            // load description from localized strings
            periodicTask.Description = AppResources.AnnouncementBackgroundTaskDescription;
            // set expiration days
            periodicTask.ExpirationTime = DateTime.Now.AddDays(14);
            try
            {
                // add thas to scheduled action service
                ScheduledActionService.Add(periodicTask);
                // debug, so run in every 30 secs

#if DEBUG
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(10));
                System.Diagnostics.Debug.WriteLine("Periodic task started: " + periodicTaskName);
#endif

            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    // load error text from localized strings
                    ShowPopup(CustomPopupMessageType.Error, AppResources.AnnouncementBackgrounTaskDisabledPopupText, AppResources.CustomPopupGenericOkGotItMessage, null);
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
            */
            ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("/Views/Announcements.xaml"));

            if (tile == null)
            {
                var newTile = new StandardTileData()
                {
                    Title = AppResources.AnnouncementsLiveTileText,
                    BackgroundImage = new Uri("/Images/Tiles/TileAnnouncement7.png", UriKind.Relative),
                };

                ShellTile.Create(new Uri("/Views/Announcements.xaml", UriKind.Relative), newTile);
            }
            else
            {
                ShowPopup(CustomPopupMessageType.Error, AppResources.TileAlreadyPinnedErrorText, AppResources.CustomPopupGenericOkGotItMessage, null);
            }
        }

        private void ShakeGesture(ShakeGestureMessage action)
        {
            LoadAnnouncements();
        }

        private void LoadAnnouncements()
        {
            if (isLoadingAnnouncements)
                return;

            isLoadingAnnouncements = true;

            IEnumerable<TransportMode> modes = UnitOfWork.TransportModeRepository.GetAll();

            if (!modes.Any(x => x.IsEnabled))
            {
                isLoadingAnnouncements = false;

                base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToNoModesErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);

                return;
            }

            IEnumerable<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll();

            if (!operatorSettings.Any(x => x.IsEnabled))
            {
                isLoadingAnnouncements = false;

                base.ShowPopup(CustomPopupMessageType.Error, AppResources.WhereToNoModesErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);

                return;
            }

            Action loadAnnouncements = async () =>
            {
                LoadingBarMessage.Send(LoadingBarMessageReason.Show);

                try
                {
                    List<string> excludedModes = modes.Where(x => x.IsEnabled == false).Select(x => x.ApplicationTransportMode.ToString()).ToList();
                    List<string> excludedOperators = operatorSettings.Where(x => x.IsEnabled == false).Select(x => x.OperatorName).ToList();

                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = new CancellationTokenSource();

                    this.Announcements.Clear();
                    this.Announcements.AddRange(await BumbleApi.Announcements(cancellationTokenSource.Token, user, excludedModes, excludedOperators));
                }
                catch (Exception e)
                {
                    if (e.Message != "Cancelled")
                        base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                }

                if (this.Announcements.Count() == 0)
                    NoResultsVisibility = Visibility.Visible;
                else
                    NoResultsVisibility = Visibility.Collapsed;

                isLoadingAnnouncements = false;
                LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
            };

            DispatcherHelper.CheckBeginInvokeOnUI(loadAnnouncements);
        }

        private void HideApplicationBar()
        {
            ApplicationBarIsVisibile = false;

            base.InMemoryApplicationSettingModel.UpdateSetting(ApplicationSetting.ShowAnnouncementsApplicationBar, false);
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

        private ObservableCollection<Announcement> announcements = new ObservableCollection<Announcement>();
        public ObservableCollection<Announcement> Announcements
        {
            get { return announcements; }
        }

        #endregion

        #region Commands

        public RelayCommand HideApplicationBarCommand
        {
            get { return new RelayCommand(HideApplicationBar); }
        }

        public RelayCommand PinAnnouncementsCommand
        {
            get { return new RelayCommand(PinAnnouncements); }
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
