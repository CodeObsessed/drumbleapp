using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using System;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class UberAuthenticationViewModel : AnalyticsBase, IDisposable
    {
        private UberOAuthCredentials credentials;
        private IUberService uberService;

        public UberAuthenticationViewModel(IAggregateService aggregateService, IUberService uberService)
            : base(ApplicationPage.UberAuthentication, aggregateService)
        {
            this.uberService = uberService;

            this.credentials = new UberOAuthCredentials("-PNH94q8gmuRmb5ZvnEVYO0P-ysj_8-_", "1h-6l1ZVRciE0suS9E5LRjbXvoXg-Q6d3lZfaFxX", UberOAuthGrantType.authorization_code);
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();
        }

        #endregion

        #region Local Functions

        private void Navigating(NavigatingEventArgs e)
        {
            if (e.Uri.AbsoluteUri.Contains("?code"))
            {
                string code = GetQueryParameter(e.Uri.AbsoluteUri.Substring(e.Uri.AbsoluteUri.IndexOf("?") + 1), "code");

                this.credentials.SetCode(code);

                Action authenticate = async () =>
                {
                    UberAuthenticationDetails response = await uberService.Authenticate(CancellationToken.None, this.credentials);

                    if (response == null)
                    {
                        // Display error
                        ShowPopup(CustomPopupMessageType.Error, AppResources.UberErrorAuthenticating, AppResources.CustomPopupGenericOkMessage, null);

                        return;
                    }

                    user.UberInfo = new UberInfo(response.AccessToken, response.RefreshToken);
                    user.IsUberAuthenticated = true;
                    user.DismissedLoginUberPopup = true;
                    UnitOfWork.UserRepository.Update(user);
                    UnitOfWork.Save();

                    InMemoryApplicationSettingModel.UpdateSetting(ApplicationSetting.LoginUber, true);

                    NavigationService.GoBack();
                };

                DispatcherHelper.CheckBeginInvokeOnUI(authenticate);

            }
        }

        private static string GetQueryParameter(string input, string parameterName)
        {
            foreach (string item in input.Split('&'))
            {
                var parts = item.Split('=');
                if (parts[0] == parameterName)
                {
                    return parts[1];
                }
            }
            return String.Empty;
        }

        #endregion

        #region Commands

        public RelayCommand<NavigatingEventArgs> NavigatingCommand
        {
            get { return new RelayCommand<NavigatingEventArgs>(Navigating); }
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
