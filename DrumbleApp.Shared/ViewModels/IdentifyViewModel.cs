using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class IdentifyViewModel : AnalyticsBase, IDisposable
    {
        private IDrumbleApiService DrumbleApi;
        private CancellationTokenSource cancellationTokenSource;
        private Email email;

        public IdentifyViewModel(IAggregateService aggregateService, IDrumbleApiService DrumbleApi)
            : base(ApplicationPage.Identify, aggregateService)
        {
            if (DrumbleApi == null)
                throw new ArgumentNullException("DrumbleApi");

            this.DrumbleApi = DrumbleApi;
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            OneTimePin = AppResources.IdentifyOneTimePinWaterMarkText;

            email = SimpleIoc.Default.GetInstance<Email>();

            SimpleIoc.Default.Unregister<Email>();

            base.ShowPopup(CustomPopupMessageType.Information, string.Format(AppResources.IdentifyOtpSentPopupText, email.EmailAddress), AppResources.CustomPopupGenericOkMessage, null);
        }

        #endregion

        #region Local Functions

        private void Cancel()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();

                OneTimePin = AppResources.IdentifyOneTimePinWaterMarkText;
                OneTimePinTextBoxIsEnabled = true;
                base.HideHeaderLoader();
            }

            IdentifyMessage.Send(null, Messages.Enums.IdentifyMessageReason.CanceledIdentification);

            NavigationService.GoBack();
        }

        private void ResendPin()
        {
            base.ShowHeaderLoader();
            OneTimePinTextBoxIsEnabled = false;

            Action resendPin = async () =>
            {
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();

                    bool otpSent = await DrumbleApi.LoginEmail(cancellationTokenSource.Token, email);

                    if (otpSent)
                    {
                        base.HideHeaderLoader();
                        OneTimePinTextBoxIsEnabled = true;

                        base.ShowPopup(CustomPopupMessageType.Information, string.Format(AppResources.IdentifyOtpSentPopupText, email.EmailAddress), AppResources.CustomPopupGenericOkMessage, null);
                    }
                    else
                    {
                        // TODO error display something went wrong... but what?
                    }
                }
                catch (Exception e)
                {
                    base.HideHeaderLoader();
                    OneTimePinTextBoxIsEnabled = true;

                    base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(resendPin);

        }

        private void Authorise()
        {
            Pin pin;

            try
            {
                pin = Pin.CreateFrom(OneTimePin);
            }
            catch (Exception e)
            {
                base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            base.ShowHeaderLoader();
            OneTimePinTextBoxIsEnabled = false;

            Action identify = async () =>
            {
                try
                {
                    cancellationTokenSource = new CancellationTokenSource();

                    user = await DrumbleApi.Identify(cancellationTokenSource.Token, email, pin);

                    if (user != null)
                    {
                        base.HideHeaderLoader();
                        OneTimePinTextBoxIsEnabled = true;

                        IdentifyMessage.Send(user, Messages.Enums.IdentifyMessageReason.Identified);

                        NavigationService.GoBack();
                    }
                    else
                    {
                        // TODO error display something went wrong... but what?
                    }
                }
                catch (Exception e)
                {
                    base.HideHeaderLoader();
                    OneTimePinTextBoxIsEnabled = true;

                    base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                }
            };

            DispatcherHelper.CheckBeginInvokeOnUI(identify);
        }


        private void OneTimePinTextBoxGotFocus()
        {
            if (OneTimePin.Equals(AppResources.IdentifyOneTimePinWaterMarkText))
            {
                OneTimePin = String.Empty;
            }
        }

        private void OneTimePinTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(OneTimePin))
            {
                OneTimePin = AppResources.IdentifyOneTimePinWaterMarkText;
            }
        }

        #endregion

        #region Properties

        private string oneTimePin = AppResources.IdentifyOneTimePinWaterMarkText;
        public string OneTimePin
        {
            get { return oneTimePin; }
            set
            {
                oneTimePin = value;
                RaisePropertyChanged("OneTimePin");
            }
        }

        private bool oneTimePinTextBoxIsEnabled = true;
        public bool OneTimePinTextBoxIsEnabled
        {
            get { return oneTimePinTextBoxIsEnabled; }
            set
            {
                oneTimePinTextBoxIsEnabled = value;
                RaisePropertyChanged("OneTimePinTextBoxIsEnabled");
            }
        }

        #endregion

        #region Commands

        public RelayCommand ResendPinCommand
        {
            get { return new RelayCommand(ResendPin); }
        }

        public RelayCommand CancelCommand
        {
            get { return new RelayCommand(Cancel); }
        }

        public RelayCommand OneTimePinTextBoxGotFocusCommand
        {
            get { return new RelayCommand(OneTimePinTextBoxGotFocus); }
        }

        public RelayCommand OneTimePinTextBoxLostFocusCommand
        {
            get { return new RelayCommand(OneTimePinTextBoxLostFocus); }
        }

        public RelayCommand AuthoriseCommand
        {
            get { return new RelayCommand(Authorise); }
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
