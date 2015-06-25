using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;

namespace DrumbleApp.Shared.Infrastructure.Analytics
{
    public abstract class AnalyticsBase : ViewModelBase
    {
        private IAggregateService AggregateService { get; set; }
        protected IUnitOfWork UnitOfWork
        {
            get
            {
                return this.AggregateService.UnitOfWork;
            }
        }
        protected INavigationService NavigationService
        {
            get
            {
                return this.AggregateService.NavigationService;
            }
        }
        protected IInMemoryApplicationSettingModel InMemoryApplicationSettingModel
        {
            get
            {
                return this.AggregateService.InMemoryApplicationSettingModel;
            }
        }
        protected User user;

        public bool SentCoordinateRequest = false;

        private ApplicationPage appPage;
        private Weather weather;

        protected AnalyticsBase(ApplicationPage appPage, IAggregateService aggregateService)
        {
            if (aggregateService == null)
                throw new ArgumentNullException("aggregateService");

            this.AggregateService = aggregateService; 
            this.appPage = appPage;
            if (appPage != ApplicationPage.SplashScreen)
                this.weather = UnitOfWork.WeatherRepository.Get();
        }

        #region Virtual

        protected virtual void PageLoaded()
        {
            this.user = UnitOfWork.UserRepository.GetUser();
        }

        protected virtual void PageUnloaded()
        {
            this.user = null;
        }

        protected virtual void UserLocationFound(GpsWatcherResponseMessage gpsWatcherResponseMessage)
        {
            if (gpsWatcherResponseMessage.Reason == GpsWatcherResponseMessageReason.Error)
            {
                DeregisterWatcher();

                this.ShowPopup(CustomPopupMessageType.Error, AppResources.CustomPopupGenericGpsFailureMessage, AppResources.CustomPopupGenericOkMessage, null);

                return;
            }

            if (SentCoordinateRequest || gpsWatcherResponseMessage.IsUsingLastKnown)
            {
                DeregisterWatcher();

                if (user != null && !gpsWatcherResponseMessage.IsUsingLastKnown)
                {
                    user.UpdateLocation(gpsWatcherResponseMessage.Coordinate);
                    UnitOfWork.UserRepository.Update(user);
                    UnitOfWork.Save();
                }

                if (InMemoryApplicationSettingModel.GetSetting(ApplicationSetting.ShowWeather).Value)
                {
                    if (weather == null || weather.LastRefreshedDate < DateTime.Now.AddHours(-1))
                    {
                        Action getWeather = async () =>
                        {
                            try
                            {
                                IWeatherApi weatherApi = SimpleIoc.Default.GetInstance<IWeatherApi>();

                                if (weatherApi != null)
                                {
                                    weather = await weatherApi.GetDataWeather(gpsWatcherResponseMessage.Coordinate);

                                    UnitOfWork.WeatherRepository.DeleteAll();
                                    UnitOfWork.WeatherRepository.Insert(weather);
                                    UnitOfWork.Save();
                                }
                            }
                            catch (Exception)
                            {

                            }
                        };

                        DispatcherHelper.CheckBeginInvokeOnUI(getWeather);
                    }
                }
            }
        }

        #endregion

        #region Public

        public void RegisterWatcher()
        {
            SentCoordinateRequest = true;

            Messenger.Default.Register<GpsWatcherResponseMessage>(this, (action) => UserLocationFound(action));

            GpsWatcherMessage.Send(GpsWatcherMessageReason.Start);

            LoadingBarMessage.Send(LoadingBarMessageReason.Show);
        }

        #endregion

        #region Private

        private void DeregisterWatcher()
        {
            SentCoordinateRequest = false;

            Messenger.Default.Unregister<GpsWatcherResponseMessage>(this);

            LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
        }

        #endregion

        #region Messaging

        protected void ShowPopup(CustomPopupMessageType type, string text, string leftButtonText, string rightButtonText)
        {
            Messenger.Default.Send<CustomPopupMessage>(new CustomPopupMessage(type, text, leftButtonText, rightButtonText));
        }

        protected void ShowPopupWithAction(CustomPopupMessageType type, string text, string leftButtonText, string rightButtonText, Action leftButtonAction, Action rightButtonAction, Action noButtonAction)
        {
            Messenger.Default.Send<CustomPopupMessageWithAction>(new CustomPopupMessageWithAction(type, text, leftButtonText, rightButtonText, leftButtonAction, rightButtonAction, noButtonAction));
        }

        protected void ShowHeaderLoader()
        {
            LoadingBarMessage.Send(LoadingBarMessageReason.Show);
        }

        protected void HideHeaderLoader()
        {
            LoadingBarMessage.Send(LoadingBarMessageReason.Hide);
        }

        #endregion

        #region Commands

        public RelayCommand PageLoadedCommand
        {
            get { return new RelayCommand(PageLoaded); }
        }

        public RelayCommand PageUnloadedCommand
        {
            get { return new RelayCommand(PageUnloaded); }
        }

        #endregion
    }
}
