using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class MessageUsViewModel : AnalyticsBase, IDisposable
    {
        private IDrumbleApiService DrumbleApi;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MessageUsViewModel(IAggregateService aggregateService, IDrumbleApiService DrumbleApi)
            : base(ApplicationPage.MessageUs, aggregateService)
        {
            if (DrumbleApi == null)
                throw new ArgumentNullException("DrumbleApi");

            this.DrumbleApi = DrumbleApi;
        }

        #region Overrides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderMessage);
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Message = String.Empty;
        }

        #endregion

        #region Local Functions

        private void Cancel()
        {
            if (MessageTextBoxIsEnabled == false)
            {
                cancellationTokenSource.Cancel();
            }

            NavigationService.GoBack();
        }

        private void Send()
        {
            if (Message.Equals(AppResources.MessageUsTextBoxWaterMark) || String.IsNullOrEmpty(Message))
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.MessageUsNoMessagePopupText, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            if (!Email.Equals(AppResources.MessageUsEmailTextBoxWaterMark))
            {
                try
                {
                    ValueObjects.Email fromEmail = new ValueObjects.Email(Email);
                }
                catch (Exception) 
                {
                    base.ShowPopup(CustomPopupMessageType.Error, AppResources.InvalidEmailAlert, AppResources.CustomPopupGenericOkMessage, null);
                    return;
                }
            }
            else if (Email.Equals(AppResources.MessageUsEmailTextBoxWaterMark))
            {
                base.ShowPopup(CustomPopupMessageType.Error, AppResources.MessageUsNoEmailPopupText, AppResources.CustomPopupGenericOkMessage, null);
                return;
            }

            Action contact = async () =>
            {
                base.ShowHeaderLoader();

                MessageTextBoxIsEnabled = false;
                try
                {
                    ValueObjects.Email fromEmail = new ValueObjects.Email(Email);

                    string messageSubject = "Drumble v2 WP Feedback: " + ((Subject.Equals(AppResources.MessageUsSubjectTextBoxWaterMark)) ? "Empty" : Subject);

                    cancellationTokenSource = new CancellationTokenSource();

                    var contactResult = await DrumbleApi.Contact(cancellationTokenSource.Token, UnitOfWork.UserRepository.GetUser(), fromEmail, messageSubject, Message + "\n\nDevice Details: " + UserAgent());

                    Message = String.Empty;
                    Email = AppResources.MessageUsEmailTextBoxWaterMark;
                    Subject = AppResources.MessageUsSubjectTextBoxWaterMark;

                    base.ShowPopup(CustomPopupMessageType.Sucess, AppResources.MessageUsSuccessPopupText, AppResources.CustomPopupGenericOkMessage, null);
                }
                catch (Exception e)
                {
                    if (e.Message != "Cancelled")
                        base.ShowPopup(CustomPopupMessageType.Error, e.Message, AppResources.CustomPopupGenericOkMessage, null);
                }

                base.HideHeaderLoader();
                MessageTextBoxIsEnabled = true;

            };

            DispatcherHelper.CheckBeginInvokeOnUI(contact);
        }

        private void SubjectTextBoxGotFocus()
        {
            if (Subject.Equals(AppResources.MessageUsSubjectTextBoxWaterMark))
            {
                Subject = String.Empty;
            }
        }

        private void SubjectTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(Subject))
            {
                Subject = AppResources.MessageUsSubjectTextBoxWaterMark;
            }
        }

        private void EmailTextBoxGotFocus()
        {
            if (Email.Equals(AppResources.MessageUsEmailTextBoxWaterMark))
            {
                Email = String.Empty;
            }
        }

        private void EmailTextBoxLostFocus()
        {
            if (String.IsNullOrEmpty(Email))
            {
                Email = AppResources.MessageUsEmailTextBoxWaterMark;
            }
        }

        private static string UserAgent()
        {
            var maker = Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer;
            var model = Microsoft.Phone.Info.DeviceStatus.DeviceName;
            return string.Format("{0} {1} {2}", maker, model, "Windows Phone " + Environment.OSVersion.Version.ToString());
        }

        #endregion

        #region Properties

        private string email = AppResources.MessageUsEmailTextBoxWaterMark;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        private string subject = AppResources.MessageUsSubjectTextBoxWaterMark;
        public string Subject
        {
            get { return subject; }
            set
            {
                subject = value;
                RaisePropertyChanged("Subject");
            }
        }

        private string message = String.Empty;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged("Message");
            }
        }

        private bool messageTextBoxIsEnabled = true;
        public bool MessageTextBoxIsEnabled
        {
            get { return messageTextBoxIsEnabled; }
            set
            {
                messageTextBoxIsEnabled = value;
                RaisePropertyChanged("MessageTextBoxIsEnabled");
            }
        }
        
        #endregion

        #region Commands

        public RelayCommand SendCommand
        {
            get { return new RelayCommand(Send); }
        }

        public RelayCommand CancelCommand
        {
            get { return new RelayCommand(Cancel); }
        }

        public RelayCommand SubjectTextBoxGotFocusCommand
        {
            get { return new RelayCommand(SubjectTextBoxGotFocus); }
        }

        public RelayCommand SubjectTextBoxLostFocusCommand
        {
            get { return new RelayCommand(SubjectTextBoxLostFocus); }
        }

        public RelayCommand EmailTextBoxGotFocusCommand
        {
            get { return new RelayCommand(EmailTextBoxGotFocus); }
        }

        public RelayCommand EmailTextBoxLostFocusCommand
        {
            get { return new RelayCommand(EmailTextBoxLostFocus); }
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
