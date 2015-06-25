using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using System;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class SettingsViewModel : AnalyticsBase, IDisposable
    {
        private bool firstLoad = true;
        private bool uberAuthenticated = false;

        public SettingsViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.Settings, aggregateService)
        {
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderSettings);

            firstLoad = true;

            LoadSettings();

            firstLoad = false;
        }

        #endregion

        #region Local Functions

        private void ChangeCountry()
        {
            NavigationService.NavigateTo("/Views/SplashScreen.xaml?pagestate=changecountry");
        }
        private void ResetApp()
        {
            NavigationService.NavigateTo("/Views/SplashScreen.xaml?pagestate=resetapp");
        }

        private void Close()
        {
            NavigationService.GoBack();
        }

        private void LoadSettings()
        {
            LocationIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value;

            UseMetricIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.UseMetric).Value;

            ShowWeatherIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowWeather).Value;

            UseUberIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.UseUber).Value;

            StoreRecentIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.StoreRecent).Value;

            PopulateLocationIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AutoPopulateLocation).Value;

            PopulateMostUsedIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AutoPopulateMostFrequent).Value;

            PopulateRecentIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AutoPopulateMostRecent).Value;

            SkipTripSelectionIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.SkipTripSelection).Value;

            ShowAnnouncementsApplicationBarIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowAnnouncementsApplicationBar).Value;

            ShowTripApplicationBarIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowTripApplicationBar).Value;

            LoginUberIsChecked = InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.LoginUber).Value;
        }

        private void SaveSetting(ApplicationSetting setting, bool value)
        {
            InMemoryApplicationSettingModel.UpdateSetting(setting, value);
        }

        private void DisplayFeedback()
        {
            base.ShowPopup(CustomPopupMessageType.Information, AppResources.SettingsSavedPopupMessage, AppResources.CustomPopupGenericOkMessage, null);
        }

        private void ShowWeather()
        {
            SaveSetting(ApplicationSetting.ShowWeather, ShowWeatherIsChecked);

            DisplayFeedback();
        }

        private void AllowLocation()
        {
            SaveSetting(ApplicationSetting.AllowLocation, LocationIsChecked);

            DisplayFeedback();
        }

        private void UseMetric()
        {
            SaveSetting(ApplicationSetting.UseMetric, UseMetricIsChecked);

            DisplayFeedback();
        }

        private void ShowAnnouncementsApplicationBar()
        {
            SaveSetting(ApplicationSetting.ShowAnnouncementsApplicationBar, ShowAnnouncementsApplicationBarIsChecked);

            DisplayFeedback();
        }

        private void ShowTripApplicationBar()
        {
            SaveSetting(ApplicationSetting.ShowTripApplicationBar, ShowTripApplicationBarIsChecked);

            DisplayFeedback();
        }

        private void StoreRecent()
        {
            SaveSetting(ApplicationSetting.StoreRecent, StoreRecentIsChecked);

            // Clear existing recent trips.
            if (!StoreRecentIsChecked)
            {
                UnitOfWork.RecentTripRepository.RemoveAll();
                UnitOfWork.Save();
            }

            DisplayFeedback();
        }

        private void PopulateMostUsed()
        {
            firstLoad = true;
            PopulateLocationIsChecked = false;
            PopulateRecentIsChecked = false;
            firstLoad = false;
            SaveSetting(ApplicationSetting.AutoPopulateLocation, false);
            SaveSetting(ApplicationSetting.AutoPopulateMostRecent, false);
            SaveSetting(ApplicationSetting.AutoPopulateMostFrequent, PopulateMostUsedIsChecked);

            DisplayFeedback();
        }

        private void PopulateLocation()
        {
            firstLoad = true;
            PopulateMostUsedIsChecked = false;
            PopulateRecentIsChecked = false;
            firstLoad = false;
            SaveSetting(ApplicationSetting.AutoPopulateMostFrequent, false);
            SaveSetting(ApplicationSetting.AutoPopulateMostRecent, false);
            SaveSetting(ApplicationSetting.AutoPopulateLocation, PopulateLocationIsChecked);

            DisplayFeedback();
        }

        private void UseUber()
        {
            SaveSetting(ApplicationSetting.UseUber, UseUberIsChecked);

            DisplayFeedback();
        }

        private void PopulateRecent()
        {
            firstLoad = true;
            PopulateMostUsedIsChecked = false;
            PopulateLocationIsChecked = false;
            firstLoad = false;
            SaveSetting(ApplicationSetting.AutoPopulateLocation, false);
            SaveSetting(ApplicationSetting.AutoPopulateMostFrequent, false);
            SaveSetting(ApplicationSetting.AutoPopulateMostRecent, PopulateRecentIsChecked);

            DisplayFeedback();
        }

        private void SkipTripSelection()
        {
            SaveSetting(ApplicationSetting.SkipTripSelection, SkipTripSelectionIsChecked);

            DisplayFeedback();
        }

        #endregion

        #region Properties

        private bool skipTripSelectionIsChecked = false;
        public bool SkipTripSelectionIsChecked
        {
            get { return skipTripSelectionIsChecked; }
            set
            {
                skipTripSelectionIsChecked = value;

                RaisePropertyChanged("SkipTripSelectionIsChecked");

                if (!firstLoad)
                    SkipTripSelection();
            }
        }

        private bool useUberIsChecked = false;
        public bool UseUberIsChecked
        {
            get { return useUberIsChecked; }
            set
            {
                useUberIsChecked = value;

                RaisePropertyChanged("UseUberIsChecked");

                if (!firstLoad)
                    UseUber();
            }
        }

        private bool loginUberIsChecked = false;
        public bool LoginUberIsChecked
        {
            get { return loginUberIsChecked; }
            set
            {
                if (!firstLoad && !uberAuthenticated)
                {
                    if (value)
                    {
                        loginUberIsChecked = false;
                        NavigationService.NavigateTo("/Views/UberAuthentication.xaml");
                    }
                    else
                    {
                        // Logout
                        user.IsUberAuthenticated = false;
                        user.UberInfo = null;
                        UnitOfWork.UserRepository.Update(user);
                        UnitOfWork.Save();

                        SaveSetting(ApplicationSetting.LoginUber, false);

                        loginUberIsChecked = false;
                    }
                }
                else
                {
                    loginUberIsChecked = value;
                    uberAuthenticated = false;
                }

                RaisePropertyChanged("LoginUberIsChecked");
            }
        }

        private bool populateLocationIsChecked = false;
        public bool PopulateLocationIsChecked
        {
            get { return populateLocationIsChecked; }
            set
            {
                populateLocationIsChecked = value;

                RaisePropertyChanged("PopulateLocationIsChecked");

                if (!firstLoad)
                    PopulateLocation();
            }
        }

        private bool populateRecentIsChecked = false;
        public bool PopulateRecentIsChecked
        {
            get { return populateRecentIsChecked; }
            set
            {
                populateRecentIsChecked = value;

                RaisePropertyChanged("PopulateRecentIsChecked");

                if (!firstLoad)
                    PopulateRecent();
            }
        }

        private bool populateMostUsedIsChecked = false;
        public bool PopulateMostUsedIsChecked
        {
            get { return populateMostUsedIsChecked; }
            set
            {
                populateMostUsedIsChecked = value;

                RaisePropertyChanged("PopulateMostUsedIsChecked");

                if (!firstLoad)
                    PopulateMostUsed();
            }
        }

        private bool storeRecentIsChecked = false;
        public bool StoreRecentIsChecked
        {
            get { return storeRecentIsChecked; }
            set
            {
                storeRecentIsChecked = value;

                RaisePropertyChanged("StoreRecentIsChecked");

                if (!firstLoad)
                    StoreRecent();
            }
        }

        private bool showWeatherIsChecked = false;
        public bool ShowWeatherIsChecked
        {
            get { return showWeatherIsChecked; }
            set
            {
                showWeatherIsChecked = value;

                RaisePropertyChanged("ShowWeatherIsChecked");

                if (!firstLoad)
                    ShowWeather();
            }
        }

        private bool locationIsChecked = false;
        public bool LocationIsChecked
        {
            get { return locationIsChecked; }
            set
            {
                locationIsChecked = value;

                RaisePropertyChanged("LocationIsChecked");

                if (!firstLoad)
                    AllowLocation();
            }
        }

        private bool useMetricIsChecked = false;
        public bool UseMetricIsChecked
        {
            get { return useMetricIsChecked; }
            set
            {
                useMetricIsChecked = value;

                RaisePropertyChanged("UseMetricIsChecked");

                if (!firstLoad)
                    UseMetric();
            }
        }

        private bool showAnnouncementsApplicationBarIsChecked = true;
        public bool ShowAnnouncementsApplicationBarIsChecked
        {
            get { return showAnnouncementsApplicationBarIsChecked; }
            set
            {
                showAnnouncementsApplicationBarIsChecked = value;

                RaisePropertyChanged("ShowAnnouncementsApplicationBarIsChecked");

                if (!firstLoad)
                    ShowAnnouncementsApplicationBar();
            }
        }

        private bool showTripApplicationBarIsChecked = true;
        public bool ShowTripApplicationBarIsChecked
        {
            get { return showTripApplicationBarIsChecked; }
            set
            {
                showTripApplicationBarIsChecked = value;

                RaisePropertyChanged("ShowTripApplicationBarIsChecked");

                if (!firstLoad)
                    ShowTripApplicationBar();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CloseCommand
        {
            get { return new RelayCommand(Close); }
        }

        public RelayCommand ResetAppCommand
        {
            get { return new RelayCommand(ResetApp); }
        }

        public RelayCommand ChangeCountryCommand
        {
            get { return new RelayCommand(ChangeCountry); }
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
