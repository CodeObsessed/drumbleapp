using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class HeaderViewModel : AnalyticsBase, IDisposable
    {
        private Weather weather;
        private Stack<int> loadingStack = new Stack<int>();

        // Temporarily dismiss popups that need to show until they are actioned by storing their state in the viewmodel for the app lifetime.
        private bool userDismissedPrivateModesPopup = false;

        public HeaderViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.Header, aggregateService)
        {
            Messenger.Default.Register<LoadingBarMessage>(this, (action) => SetLoadingBarState(action));

            Messenger.Default.Register<PageTitleMessage>(this, (action) => SetPageTitle(action));
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            SetPopupVisibilities();

            if (InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowWeather).Value)
            {
                if (weather == null || weather.LastRefreshedDate < DateTime.Now.AddHours(-1))
                {
                    weather = UnitOfWork.WeatherRepository.Get();
                }

                if (weather != null)
                {
                    WeatherIconImageSource = new BitmapImage(new Uri(weather.Icon, UriKind.Relative));
                    WeatherIconImageVisibility = Visibility.Visible;
                }
            }
            else
            {
                WeatherIconImageVisibility = Visibility.Collapsed;
            }
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();
        }

        #endregion

        #region Properties

        private bool loaderProgressBarIsIndeterminate = false;
        public bool LoaderProgressBarIsIndeterminate
        {
            get { return loaderProgressBarIsIndeterminate; }
            set
            {
                loaderProgressBarIsIndeterminate = value;
                RaisePropertyChanged("LoaderProgressBarIsIndeterminate");
            }
        }

        private Visibility loaderProgressBarVisibility = Visibility.Collapsed;
        public Visibility LoaderProgressBarVisibility
        {
            get { return loaderProgressBarVisibility; }
            set
            {
                loaderProgressBarVisibility = value;
                RaisePropertyChanged("LoaderProgressBarVisibility");
            }
        }

        private Visibility weatherIconImageVisibility = Visibility.Collapsed;
        public Visibility WeatherIconImageVisibility
        {
            get { return weatherIconImageVisibility; }
            set
            {
                weatherIconImageVisibility = value;
                RaisePropertyChanged("WeatherIconImageVisibility");
            }
        }

        private BitmapImage weatherIconImageSource;
        public BitmapImage WeatherIconImageSource
        {
            get { return weatherIconImageSource; }
            set
            {
                weatherIconImageSource = value;
                RaisePropertyChanged("WeatherIconImageSource");
            }
        }

        private string pageTitleText;
        public string PageTitleText
        {
            get { return pageTitleText; }
            set
            {
                pageTitleText = value;
                RaisePropertyChanged("PageTitleText");
            }
        }

        private string privateModePopupText;
        public string PrivateModePopupText
        {
            get { return privateModePopupText; }
            set
            {
                privateModePopupText = value;
                RaisePropertyChanged("PrivateModePopupText");
            }
        }

        private Visibility privateModesPopupVisibility = Visibility.Collapsed;
        public Visibility PrivateModesPopupVisibility
        {
            get { return privateModesPopupVisibility; }
            set
            {
                privateModesPopupVisibility = value;
                RaisePropertyChanged("PrivateModesPopupVisibility");
            }
        }

        private Visibility gpsPopupVisibility = Visibility.Collapsed;
        public Visibility GpsPopupVisibility
        {
            get
            {
                return gpsPopupVisibility;
            }
            set
            {
                gpsPopupVisibility = value;

                RaisePropertyChanged("GpsPopupVisibility");
            }
        }

        private Visibility rateAppPopupVisibility = Visibility.Collapsed;
        public Visibility RateAppPopupVisibility
        {
            get { return rateAppPopupVisibility; }
            set
            {
                rateAppPopupVisibility = value;
                RaisePropertyChanged("RateAppPopupVisibility");
            }
        }

        private Visibility loginUberPopupVisibility = Visibility.Collapsed;
        public Visibility LoginUberPopupVisibility
        {
            get { return loginUberPopupVisibility; }
            set
            {
                loginUberPopupVisibility = value;
                RaisePropertyChanged("LoginUberPopupVisibility");
            }
        }

        #endregion

        #region Functions

        private void SetPageTitle(PageTitleMessage action)
        {
            this.PageTitleText = action.PageTitle;
        }

        private void SetLoadingBarState(LoadingBarMessage loadingBarMessage)
        {
            switch (loadingBarMessage.Reason)
            {
                case DrumbleApp.Shared.Messages.Enums.LoadingBarMessageReason.Show:
                    loadingStack.Push(1);
                    if (!LoaderProgressBarIsIndeterminate)
                    {
                        LoaderProgressBarIsIndeterminate = true;
                        LoaderProgressBarVisibility = Visibility.Visible;
                    }
                    break;
                case DrumbleApp.Shared.Messages.Enums.LoadingBarMessageReason.Hide:
                    if (loadingStack.Count > 0)
                        loadingStack.Pop();
                    if (loadingStack.Count == 0)
                    {
                        LoaderProgressBarIsIndeterminate = false;
                        LoaderProgressBarVisibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        private void SetPopupVisibilities()
        {
            HidePopups();

            if (base.user != null)
            {
                if (!user.DismissedLocationPopup)
                {
                    if (!InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.AllowLocation).Value)
                    {
                        GpsPopupVisibility = Visibility.Visible;
                        return;
                    }
                }

                if (!userDismissedPrivateModesPopup)
                {
                    List<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll().ToList();

                    int numberUnconfiguredPrivateOperators = 0;
                    foreach (PublicTransportOperator publicTransportOperator in UnitOfWork.PublicTransportOperatorRepository.GetAll().ToList().Where(x => !x.IsPublic))
                    {
                        if (!operatorSettings.Where(x => x.OperatorName == publicTransportOperator.Name).FirstOrDefault().HasBeenModified)
                        {
                            numberUnconfiguredPrivateOperators += 1;
                        }
                    }

                    if (numberUnconfiguredPrivateOperators > 0)
                    {
                        PrivateModesPopupVisibility = Visibility.Visible;
                        PrivateModePopupText = string.Format(AppResources.ModePrivateTransportPopupText, numberUnconfiguredPrivateOperators.ToString());
                        return;
                    }
                }

                if (!user.DismissedLoginUberPopup)
                {
                    LoginUberPopupVisibility = Visibility.Visible;
                    return;
                }

                if (!user.DismissedRateAppPopup && user.AppUsageCount > 5)
                {
                    RateAppPopupVisibility = Visibility.Visible;
                    return;
                }
            }
        }

        private void HidePopups()
        {
            GpsPopupVisibility = Visibility.Collapsed;
            PrivateModesPopupVisibility = Visibility.Collapsed;
            RateAppPopupVisibility = Visibility.Collapsed;
            LoginUberPopupVisibility = Visibility.Collapsed;
        }

        private void RateApp()
        {
            HideRateAppPopup();

            new MarketplaceReviewTask().Show();
        }

        private void LoginUber()
        {
            HideLoginUber();

            NavigationService.NavigateTo("/Views/UberAuthentication.xaml");
        }

        private void HideRateApp()
        {
            HideRateAppPopup();
        }

        private void HideLoginUber()
        {
            HideLoginUberPopup();
        }

        private void HideLoginUberPopup()
        {
            user.DismissedLoginUberPopup = true;
            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();

            SetPopupVisibilities();
        }

        private void HideRateAppPopup()
        {
            user.DismissedRateAppPopup = true;
            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();

            SetPopupVisibilities();
        }

        private void DismissSignUpPopup()
        {
            user.DismissedSignUpPopup = true;
            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();

            SetPopupVisibilities();
        }

        private void HideSignUp()
        {
            DismissSignUpPopup();
        }

        private void SignUp()
        {
            DismissSignUpPopup();

            NavigationService.NavigateTo("/Views/Register.xaml");
        }

        private void HideGps()
        {
            user.DismissedLocationPopup = true;
            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();

            SetPopupVisibilities();
        }

        private void EnableGps()
        {
            InMemoryApplicationSettingModel.UpdateSetting(ApplicationSetting.AllowLocation, true);

            this.ShowPopup(CustomPopupMessageType.Information, AppResources.SettingsSavedPopupMessage, AppResources.CustomPopupGenericOkMessage, null);

            SetPopupVisibilities();
        }

        private void ShowModePageViaPopup()
        {
            PrivateModesPopupVisibility = Visibility.Collapsed;

            List<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll().Where(x => !x.HasBeenModified).ToList();
            foreach (OperatorSetting operatorSetting in operatorSettings)
            {
                operatorSetting.HasBeenModified = true;
                UnitOfWork.OperatorSettingRepository.Update(operatorSetting);
                UnitOfWork.Save();
            }

            if (NavigationService.CurrentPage() != "/Views/Modes.xaml")
            {
                PageTitleMessage.Send(AppResources.HeaderMode);

                NavigationService.NavigateTo("/Views/Modes.xaml");
            }
            else
                SetPopupVisibilities();
        }

        private void HidePrivateModes()
        {
            userDismissedPrivateModesPopup = true;

            SetPopupVisibilities();
        }

        #endregion

        #region Commands

        public RelayCommand LoginUberCommand
        {
            get { return new RelayCommand(LoginUber); }
        }

        public RelayCommand RateAppCommand
        {
            get { return new RelayCommand(RateApp); }
        }

        public RelayCommand HideRateAppCommand
        {
            get { return new RelayCommand(HideRateApp); }
        }

        public RelayCommand PrivateModesCommand
        {
            get { return new RelayCommand(ShowModePageViaPopup); }
        }

        public RelayCommand HidePrivateModesCommand
        {
            get { return new RelayCommand(HidePrivateModes); }
        }

        public RelayCommand SignUpCommand
        {
            get { return new RelayCommand(SignUp); }
        }

        public RelayCommand HideSignUpCommand
        {
            get { return new RelayCommand(HideSignUp); }
        }

        public RelayCommand EnableGpsCommand
        {
            get { return new RelayCommand(EnableGps); }
        }

        public RelayCommand HideGpsCommand
        {
            get { return new RelayCommand(HideGps); }
        }

        public RelayCommand HideLoginUberCommand
        {
            get { return new RelayCommand(HideLoginUber); }
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
