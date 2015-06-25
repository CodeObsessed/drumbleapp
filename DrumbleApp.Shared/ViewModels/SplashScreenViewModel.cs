using DrumbleApp.Shared.Data.Configuration;
using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Tasks;
using System.Threading;
using DrumbleApp.Shared.Messages.Classes;
using GalaSoft.MvvmLight.Ioc;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class SplashScreenViewModel : AnalyticsBase, IDisposable
    {
        private IDrumbleApiService DrumbleApiService;
        private IBumbleApiService BumbleApiService;

        private bool databaseCreated = false;
        private bool resourcesLoaded = false;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SplashScreenViewModel(IAggregateService aggregateService, IDrumbleApiService DrumbleApiService, IBumbleApiService BumbleApiService)
            : base(ApplicationPage.SplashScreen, aggregateService)
        {
            this.DrumbleApiService = DrumbleApiService;
            this.BumbleApiService = BumbleApiService;

            Messenger.Default.Register<SplashScreenMessage>(this, (action) => SetPageState(action));

            // Seed database.
            databaseCreated = DatabaseSetup.Seed(aggregateService.UnitOfWork);
        }

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            if (!resourcesLoaded)
            {
                LoadResources();
                resourcesLoaded = true;
            }
        }

        #endregion

        #region Local Functions

        private void SignIn()
        {
            NavigationService.NavigateTo("/Views/Login.xaml?pagestate=splash");
        }

        private void CountrySelectButton()
        {
            NavigationService.NavigateTo("/Views/CountrySelection.xaml");
        }

        private void LoadResources()
        {
            Action loadResources = async () =>
            {
                User user = UnitOfWork.UserRepository.GetUser();

                if (user == null)
                {
                    // App used for the first time.
                    ShowLoader(AppResources.SplashLoaderTextRegistration);

                    CountrySelection = AppResources.SplashButtonTextSelectCountry;

                    try
                    {
                        cancellationTokenSource = new CancellationTokenSource();

                        LoaderText = AppResources.SplashLoaderTextCountries;

                        Countries.Clear();
                        Countries.AddRange(await BumbleApiService.Countries());

                        ShowCountrySelector();
                    }
                    catch (Exception e)
                    {
                        base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);

                        ShowRetryResources();
                    }
                }
                else if (user.Country == null)
                {
                    // Changed country.
                    ShowLoader(AppResources.SplashLoaderTextRegistration);

                    CountrySelection = AppResources.SplashButtonTextSelectCountry;

                    try
                    {
                        LoaderText = AppResources.SplashLoaderTextCountries;

                        Countries.Clear();
                        Countries.AddRange(await BumbleApiService.Countries());

                        ShowCountrySelector();
                    }
                    catch (Exception e)
                    {
                        base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);

                        ShowRetryResources();
                    }
                }
                else
                {
                    ShowLoader(AppResources.SplashLoaderTextCheckingForUpdates);

                    SetUserPopupStates(user);

                    // App loaded, check cache if expired.
                    foreach (CacheSetting cacheSetting in UnitOfWork.CacheSettingRepository.GetAllCacheSettings())
                    {
                        if (cacheSetting.LastRefreshedDateUtc == null || cacheSetting.LastRefreshedDateUtc.Value.AddSeconds(cacheSetting.CacheValidDurationInSeconds) < DateTime.UtcNow)
                        {
                            switch (cacheSetting.ResourceType)
                            {
                                case ResourceType.Operators:
                                    try
                                    {
                                        await DownloadCountryData(user.Country, cacheSetting.LastRefreshedDateUtc);
                                    }
                                    catch (Exception e)
                                    {
                                        base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);

                                        ShowRetryResources();

                                        return;
                                    }
                                    break;
                            }
                        }
                    }

                    //LoaderText = AppResources.SplashLoaderTextComplete;

                    TryNavigateToStartPage();
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(loadResources);
        }

        private async Task DownloadCountryData(Country country, DateTime? lastUpdatedDate)
        {
            ShowLoader(String.Format(AppResources.SplashLoaderTextDownloadingCountryData, country.Name));

            if (user == null)
            {
                Token registerAnonymousToken = await DrumbleApiService.RegisterAnonymous(cancellationTokenSource.Token);

                user = new User(registerAnonymousToken);

                UnitOfWork.UserRepository.Insert(user);

                UnitOfWork.Save();
            }

            IEnumerable<PublicTransportOperator> operators = await BumbleApiService.Operators(cancellationTokenSource.Token, user);

            UnitOfWork.PublicTransportOperatorRepository.RemoveAll();

            IEnumerable<PublicTransportOperator> operatorsList = operators.ToList();

            if (operatorsList.Count() == 0)
            {
                throw new Exception(AppResources.ApiErrorPopupMessageNoResources);
            }

            List<OperatorSetting> operatorSettings = UnitOfWork.OperatorSettingRepository.GetAll().ToList();
            foreach (PublicTransportOperator transportOperator in operatorsList)
            {
                if (!operatorSettings.Select(x => x.OperatorName).Contains(transportOperator.Name))
                {
                    if (transportOperator.IsPublic)
                    {
                        UnitOfWork.OperatorSettingRepository.Insert(new OperatorSetting(transportOperator.Name, true));
                    }
                    else
                    {
                        UnitOfWork.OperatorSettingRepository.Insert(new OperatorSetting(transportOperator.Name, false));
                    }
                }
            }

            UnitOfWork.PublicTransportOperatorRepository.InsertRange(operatorsList);

            UnitOfWork.Save();

            user.Country = country;

            UnitOfWork.UserRepository.Update(user);

            UnitOfWork.Save();

            CacheSetting cacheSetting = UnitOfWork.CacheSettingRepository.GetByType(ResourceType.Operators);
            cacheSetting.LastRefreshedDateUtc = DateTime.UtcNow;

            UnitOfWork.CacheSettingRepository.Update(cacheSetting);

            UnitOfWork.Save();

            HideLoader();

            TryNavigateToStartPage();
        }

        private void HideLoader()
        {
            RetryResourcesVisibility = Visibility.Collapsed;
            CountrySelectorVisibility = Visibility.Collapsed;
            LoaderVisibility = Visibility.Collapsed;
            LoaderIsIndeterminate = false;
        }

        private void ShowLoader(string loaderText)
        {
            RetryResourcesVisibility = Visibility.Collapsed;
            CountrySelectorVisibility = Visibility.Collapsed;
            LoaderVisibility = Visibility.Visible;
            LoaderIsIndeterminate = true;
            LoaderText = loaderText;
        }

        private void ShowCountrySelector()
        {
            RetryResourcesVisibility = Visibility.Collapsed;
            CountrySelectorVisibility = Visibility.Visible;
            LoaderVisibility = Visibility.Collapsed;
            LoaderIsIndeterminate = false;
            LoaderText = String.Empty;
        }

        private void ShowRetryResources()
        {
            RetryResourcesVisibility = Visibility.Visible;
            CountrySelectorVisibility = Visibility.Collapsed;
            LoaderVisibility = Visibility.Collapsed;
            LoaderIsIndeterminate = false;
            LoaderText = String.Empty;
        }

        private void PoweredBy()
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(AppResources.CompanyWebsiteLink, UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void RetryResources()
        {
            LoadResources();
        }

        private void TryNavigateToStartPage()
        {
            // Navigation service can be null on viewmodel load as it hasn't been populated yet.
            if (NavigationService != null)
            {
                Action navigate = () =>
                {
                    user.AppUsageCount += 1;
                    UnitOfWork.UserRepository.Update(user);
                    UnitOfWork.Save();

                    NavigationService.NavigateTo("/Views/WhereTo.xaml?userId=" + user.Token.ToString());

                    AppCommandMessage.Send(Messages.Enums.AppCommandMessageReason.RemoveBackEntries);
                };

                //Keep trying to navgiate every half a second:
                TaskEx.Delay(500).ContinueWith(t =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(navigate);
                });
            }
            else
            {
                Action navigate = () =>
                {
                    TryNavigateToStartPage();
                };

                //Keep trying to navgiate every half a second:
                TaskEx.Delay(500).ContinueWith(t =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(navigate);
                });
            }
        }

        private void SetUserPopupStates(User user)
        {
            user.DismissedLocationPopup = false;

            UnitOfWork.UserRepository.Update(user);
            UnitOfWork.Save();
        }

        private void DownloadCountryData()
        {
            Action loadResources = async () =>
            {
                try
                {
                    await DownloadCountryData(selectedCountryPersist, null);
                }
                catch (Exception e)
                {
                    base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);

                    ShowRetryResources();
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(loadResources);
        }

        #endregion

        #region Local Messenger Functions

        private void SetPageState(SplashScreenMessage splashScreenMessage)
        {
            switch (splashScreenMessage.Reason)
            {
                case Messages.Enums.SplashScreenMessageReason.CountrySelected:
                    CountrySelection = selectedCountryPersist.Name;

                    NavigationService.GoBack();

                    DownloadCountryData();
                    break;
                case Messages.Enums.SplashScreenMessageReason.FacebookLogin:
                case Messages.Enums.SplashScreenMessageReason.BumbleLogin:
                case Messages.Enums.SplashScreenMessageReason.TwitterLogin:
                    base.user = UnitOfWork.UserRepository.GetUser();
                    selectedCountryPersist = user.Country;

                    DownloadCountryData();
                    break;
                case Messages.Enums.SplashScreenMessageReason.ChangeCountry:
                    AppCommandMessage.Send(Messages.Enums.AppCommandMessageReason.RemoveBackEntries);

                    base.user = UnitOfWork.UserRepository.GetUser();
                    base.user.Country = null;
                    UnitOfWork.UserRepository.Update(base.user);
                    UnitOfWork.Save();
                    resourcesLoaded = false;
                    break;
                case Messages.Enums.SplashScreenMessageReason.ResetApp:
                    AppCommandMessage.Send(Messages.Enums.AppCommandMessageReason.RemoveBackEntries);

                    DatabaseSetup.Seed(UnitOfWork, true);

                    resourcesLoaded = false;
                    break;
            }
        }

        #endregion

        #region Static Functions

        private void ContinueWithSelectedCountry()
        {
            SplashScreenMessage.Send(Messages.Enums.SplashScreenMessageReason.CountrySelected);
        }

        #endregion

        #region Properties

        private Country selectedCountry;
        private Country selectedCountryPersist;
        public Country SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                selectedCountry = value;
                RaisePropertyChanged("SelectedCountry");

                if (selectedCountry != null)
                {
                    selectedCountryPersist = selectedCountry;
                    Messenger.Default.Send<CustomPopupMessageWithAction>(new CustomPopupMessageWithAction(CustomPopupMessageType.Information, string.Format(AppResources.CountrySelectionConfirmationPopupText, value.Name), AppResources.CustomPopupGenericYesMessage, AppResources.CustomPopupGenericNoMessage, ContinueWithSelectedCountry, null, null));
                    SelectedCountry = null;
                }
            }
        }

        private Visibility retryResourcesVisibility = Visibility.Collapsed;
        public Visibility RetryResourcesVisibility
        {
            get { return retryResourcesVisibility; }
            set
            {
                retryResourcesVisibility = value;
                RaisePropertyChanged("RetryResourcesVisibility");
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

        private Visibility countrySelectorVisibility = Visibility.Collapsed;
        public Visibility CountrySelectorVisibility
        {
            get { return countrySelectorVisibility; }
            set
            {
                countrySelectorVisibility = value;
                RaisePropertyChanged("CountrySelectorVisibility");
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

        private string loaderText;
        public string LoaderText
        {
            get { return loaderText; }
            set
            {
                loaderText = value;
                RaisePropertyChanged("LoaderText");
            }
        }

        private string countrySelection = AppResources.SplashButtonTextSelectCountry;
        public string CountrySelection
        {
            get { return countrySelection; }
            set
            {
                countrySelection = value;
                RaisePropertyChanged("CountrySelection");
            }
        }

        private ObservableCollection<Country> countries = new ObservableCollection<Country>();
        public ObservableCollection<Country> Countries
        {
            get { return countries; }
        }

        #endregion

        #region Commands

        public RelayCommand SignInCommand
        {
            get { return new RelayCommand(SignIn); }
        }

        public RelayCommand RetryResourcesCommand
        {
            get { return new RelayCommand(RetryResources); }
        }

        public RelayCommand CountrySelectButtonCommand
        {
            get { return new RelayCommand(CountrySelectButton); }
        }

        public RelayCommand PoweredByCommand
        {
            get { return new RelayCommand(PoweredBy); }
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
