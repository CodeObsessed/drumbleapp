using Facebook;
using Facebook.Client;
using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class LoginViewModel : AnalyticsBase, IDisposable
    {
        private IDrumbleApiService DrumbleApi;
        private CancellationTokenSource cancellationTokenSource;
        private bool isInAppLogin = false;

        public LoginViewModel(IAggregateService aggregateService, IDrumbleApiService DrumbleApi)
            : base(ApplicationPage.Login, aggregateService)
        {
            if (DrumbleApi == null)
                throw new ArgumentNullException("DrumbleApi");

            this.DrumbleApi = DrumbleApi;

            Messenger.Default.Register<LoginMessage>(this, (action) => PageState(action));
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            Email = AppResources.LoginEmailTextBoxWaterMark;

            Messenger.Default.Unregister<IdentifyMessage>(this);
            Messenger.Default.Unregister<TwitterAccessMessage>(this);
        }

        #endregion

        #region Local Functions

        private void PageState(LoginMessage loginMessage)
        {
            switch (loginMessage.Reason)
            {
                case Messages.Enums.LoginMessageReason.InApp:
                    isInAppLogin = true;
                    break;
                case Messages.Enums.LoginMessageReason.Splash:
                    isInAppLogin = false;
                    break;
            }
        }

        private void SetEmailDetails(IdentifyMessage identifyMessage)
        {
            switch (identifyMessage.Reason)
            {
                case Messages.Enums.IdentifyMessageReason.CanceledIdentification:

                    break;
                case Messages.Enums.IdentifyMessageReason.Identified:
                    user = identifyMessage.User;

                    if (isInAppLogin)
                        UnitOfWork.UserRepository.Update(user);
                    else
                        UnitOfWork.UserRepository.Insert(user);
                    UnitOfWork.Save();

                    base.HideHeaderLoader();
                    EmailTextBoxIsEnabled = true;

                    if (isInAppLogin)
                        NavigationService.NavigateTo("/Views/Profile.xaml");
                    else
                        NavigationService.NavigateTo("/Views/SplashScreen.xaml?pagestate=Bumblelogin");

                    break;
            }

        }

        private void SetTwitterDetails(TwitterAccessMessage twitterAccessMessage)
        {
            switch (twitterAccessMessage.Reason)
            {
                case Messages.Enums.TwitterAccessMessageReason.Authorised:
                    Action registerUserTwitter = async () =>
                        {
                            if (user == null)
                                user = new User();

                            user.TwitterInfo = new TwitterInfo(twitterAccessMessage.TwitterAccess.AccessToken, twitterAccessMessage.TwitterAccess.AccessTokenSecret, twitterAccessMessage.TwitterAccess.UserId);

                            cancellationTokenSource = new CancellationTokenSource();

                            user = await DrumbleApi.LoginTwitter(cancellationTokenSource.Token, user);

                            if (isInAppLogin)
                                UnitOfWork.UserRepository.Update(user);
                            else
                                UnitOfWork.UserRepository.Insert(user);
                            UnitOfWork.Save();

                            base.HideHeaderLoader();
                            EmailTextBoxIsEnabled = true;

                            if (isInAppLogin)
                                NavigationService.NavigateTo("/Views/Profile.xaml");
                            else
                                NavigationService.NavigateTo("/Views/SplashScreen.xaml?pagestate=twitterlogin");
                        };

                        DispatcherHelper.CheckBeginInvokeOnUI(registerUserTwitter);

                    break;
                case Messages.Enums.TwitterAccessMessageReason.FailedAuthorisation:
                case Messages.Enums.TwitterAccessMessageReason.CanceledAuthorisation:
                    base.HideHeaderLoader();
                    EmailTextBoxIsEnabled = true;
                    break;
            }
        }

        private void Close()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();

                Email = AppResources.LoginEmailTextBoxWaterMark;
                EmailTextBoxIsEnabled = true;
                base.HideHeaderLoader();
            }

            NavigationService.GoBack();
        }

        private void RequestPin()
        {
            if (Email.Equals(AppResources.LoginEmailTextBoxWaterMark) || String.IsNullOrEmpty(Email))
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.LoginNoEmailErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            Email email;

            try
            {
                email = new Email(Email);
            }
            catch (Exception)
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.LoginNoEmailErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            base.ShowHeaderLoader();
            EmailTextBoxIsEnabled = false;

            Action registerEmail = async () =>
            {   
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();

                    bool otpSent = await DrumbleApi.LoginEmail(cancellationTokenSource.Token, email);

                    if (otpSent)
                    {
                        SimpleIoc.Default.Unregister<Email>();

                        SimpleIoc.Default.Register<Email>(() =>
                        {
                            return email;
                        });

                        base.HideHeaderLoader();
                        EmailTextBoxIsEnabled = true;

                        Messenger.Default.Register<IdentifyMessage>(this, (action) => SetEmailDetails(action));

                        NavigationService.NavigateTo("/Views/Identify.xaml");
                    }
                    else
                    {
                        // TODO error display something went wrong... but what?
                    }
                }
                catch (Exception e)
                {
                    base.HideHeaderLoader();
                    EmailTextBoxIsEnabled = true;

                    base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(registerEmail);
        }

        private void Facebook()
        {
            Action facebookLogin = async () =>
            {
                try
                {
                    base.ShowHeaderLoader();
                    EmailTextBoxIsEnabled = false;

                    FacebookSession facebookSession = await DrumbleApp.Shared.Infrastructure.Helpers.Facebook.Authenticate();

                    var fb = new FacebookClient(facebookSession.AccessToken);

                    fb.GetCompleted += (o, e) =>
                    {
                        if (e.Error != null)
                        {
                            Action facebookError = () =>
                            {
                                base.ShowPopup(CustomPopupMessageType.Error, AppResources.RegisterFacebookErrorPopupText, AppResources.CustomPopupGenericOkMessage, null);

                                EmailTextBoxIsEnabled = true;
                                base.HideHeaderLoader();
                            };

                            DispatcherHelper.CheckBeginInvokeOnUI(facebookError);

                            return;
                        }

                        Action getFacebookDetails = async () =>
                        {
                            var result = (IDictionary<string, object>)e.GetResultData();

                            if (user == null)
                                user = new User();

                            user.FacebookInfo = new FacebookInfo(facebookSession.AccessToken, facebookSession.FacebookId);

                            cancellationTokenSource = new CancellationTokenSource();

                            user = await DrumbleApi.LoginFacebook(cancellationTokenSource.Token, user);

                            if (isInAppLogin)
                                UnitOfWork.UserRepository.Update(user);
                            else
                                UnitOfWork.UserRepository.Insert(user);
                            UnitOfWork.Save();

                            base.HideHeaderLoader();
                            EmailTextBoxIsEnabled = true;

                            if (isInAppLogin)
                                NavigationService.NavigateTo("/Views/Profile.xaml");
                            else
                                NavigationService.NavigateTo("/Views/SplashScreen.xaml?pagestate=facebooklogin");
                        };
                        DispatcherHelper.CheckBeginInvokeOnUI(getFacebookDetails);
                    };

                    fb.GetAsync("me");
                }
                catch (Exception e)
                {
                    string exception = e.Message;

                    EmailTextBoxIsEnabled = true;
                    base.HideHeaderLoader();
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(facebookLogin);
        }

        private void Twitter()
        {
            base.ShowHeaderLoader();
            EmailTextBoxIsEnabled = false;

            Messenger.Default.Register<TwitterAccessMessage>(this, (action) => SetTwitterDetails(action));

            NavigationService.NavigateTo("/Views/TwitterAuthPage.xaml");
        }

        private void EmailTextBoxGotFocus()
        {
            if (Email.Equals(AppResources.LoginEmailTextBoxWaterMark))
            {
                Email = String.Empty;
            }
        }

        private void EmailTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(Email))
            {
                Email = AppResources.LoginEmailTextBoxWaterMark;
            }
        }

        #endregion

        #region Properties

        private string email = AppResources.LoginEmailTextBoxWaterMark;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        private bool emailTextBoxIsEnabled = true;
        public bool EmailTextBoxIsEnabled
        {
            get { return emailTextBoxIsEnabled; }
            set
            {
                emailTextBoxIsEnabled = value;
                RaisePropertyChanged("EmailTextBoxIsEnabled");
            }
        }
        
        #endregion

        #region Commands

        public RelayCommand RequestPinCommand
        {
            get { return new RelayCommand(RequestPin); }
        }

        public RelayCommand CloseCommand
        {
            get { return new RelayCommand(Close); }
        }

        public RelayCommand EmailTextBoxGotFocusCommand
        {
            get { return new RelayCommand(EmailTextBoxGotFocus); }
        }

        public RelayCommand EmailTextBoxLostFocusCommand
        {
            get { return new RelayCommand(EmailTextBoxLostFocus); }
        }

        public RelayCommand FacebookCommand
        {
            get { return new RelayCommand(Facebook); }
        }

        public RelayCommand TwitterCommand
        {
            get { return new RelayCommand(Twitter); }
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
