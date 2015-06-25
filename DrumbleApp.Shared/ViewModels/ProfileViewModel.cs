using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class ProfileViewModel : AnalyticsBase, IDisposable
    {
        private IDrumbleApiService DrumbleApi;
        private CancellationTokenSource cancellationTokenSource;

        public ProfileViewModel(IAggregateService aggregateService, IDrumbleApiService DrumbleApi)
            : base(ApplicationPage.Profile, aggregateService)
        {
            if (DrumbleApi == null)
                throw new ArgumentNullException("DrumbleApi");

            this.DrumbleApi = DrumbleApi;
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            if (!base.user.IsBumbleRegistered)
            {
                Messenger.Default.Register<ProfileMessage>(this, (action) => PageState(action));

                base.ShowPopupWithAction(CustomPopupMessageType.Information, AppResources.ProfileNotRegisteredPopupText, AppResources.ProfileRegisterPopupButtonText, AppResources.ProfileLoginPopupButtonText, Register, Login, GoBack);
            }
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Messenger.Default.Unregister<ProfileMessage>(this);
        }
        
        #endregion

        #region Local Functions

        private void PageState(ProfileMessage profileMessage)
        {
            switch (profileMessage.Reason)
            {
                case Messages.Enums.ProfileMessageReason.Register:
                    NavigationService.NavigateTo("/Views/Register.xaml");
                    NavigationService.RemoveBackEntry();
                    break;
                case Messages.Enums.ProfileMessageReason.Login:
                    NavigationService.NavigateTo("/Views/Login.xaml?pagestate=inapp");
                    NavigationService.RemoveBackEntry();
                    break;
                case Messages.Enums.ProfileMessageReason.GoBack:
                    NavigationService.GoBack();
                    break;
            }
        }

        private static void Register()
        {
            ProfileMessage.Send(Messages.Enums.ProfileMessageReason.Register);
        }

        private static void Login()
        {
            ProfileMessage.Send(Messages.Enums.ProfileMessageReason.Login);
        }

        private static void GoBack()
        {
            ProfileMessage.Send(Messages.Enums.ProfileMessageReason.GoBack);
        }

        #endregion

        #region Properties

        
        
        #endregion

        #region Commands


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
