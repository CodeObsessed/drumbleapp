using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Models;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class UberTripDetailsViewModel : AnalyticsBase, IDisposable
    {
        private DispatcherTimer timer;
        private IUberService uberService;
        private UberTripOptionModel uberTripOptionModel;

        public UberTripDetailsViewModel(IAggregateService aggregateService, IUberService uberService)
            : base(ApplicationPage.UberTripDetails, aggregateService)
        {
            this.uberService = uberService;

            this.timer = new DispatcherTimer();
            this.timer.Tick += TimerTick;
        }

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            RetryOrCancelVisibility = Visibility.Collapsed;
            TripDetailsVisibility = Visibility.Collapsed;
            FinishCancelVisibility = Visibility.Collapsed;
            CancelVisibility = Visibility.Collapsed;

            PageTitleMessage.Send(AppResources.HeaderUberTripDetails);

            if (!SimpleIoc.Default.IsRegistered<Entities.UberTrip>())
            {
                TripStatus = AppResources.UberTripStatusRequesting;

                NavigationService.RemoveBackEntry();

                this.uberTripOptionModel = SimpleIoc.Default.GetInstance<UberTripOptionModel>();

                LocationText = uberTripOptionModel.Location.ShortAddressText;
                DestinationText = uberTripOptionModel.Destination.ShortAddressText;

                // Clear any previously saved uber trips.
                UnitOfWork.UberTripRepository.RemoveAll();
                UnitOfWork.Save();

                RequestUber();
            }
            else
            {
                Entities.UberTrip uberTrip = SimpleIoc.Default.GetInstance<Entities.UberTrip>();

                if (this.UberRequest == null)
                {
                    TripStatus = AppResources.UberTripStatusProcessing;

                    LocationText = uberTrip.Location;
                    DestinationText = uberTrip.Destination;

                    this.UberRequest = new UberRequest(uberTrip.RequestId);
                }

                this.timer.Interval = new TimeSpan(0, 0, 1);
                this.timer.Start();
            }
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();
            
            this.timer.Stop();
        }

        #endregion

        #region Local Functions

        private void RequestUber()
        {
            RetryOrCancelVisibility = Visibility.Collapsed;
            TripDetailsVisibility = Visibility.Collapsed;
            FinishCancelVisibility = Visibility.Collapsed;
            CancelVisibility = Visibility.Collapsed;

            ShowHeaderLoader();

            Action requestUber = async () =>
            {
                try
                {
                    this.UberRequest = await uberService.PostUberRequest(CancellationToken.None, user.UberInfo.AccessToken, uberTripOptionModel.UberOption.ProductId, uberTripOptionModel.Location.Location, uberTripOptionModel.Destination.Location, uberTripOptionModel.SurgeConfirmationId);

                    if (UberRequest == null)
                    {
                        // Something went wrong.
                        ShowPopup(CustomPopupMessageType.Information, AppResources.ApiErrorPopupMessageUnknownError, AppResources.CustomPopupGenericOkMessage, null);

                        TripStatus = AppResources.UberTripStatusError;

                        RetryOrCancelVisibility = Visibility.Visible;
                    }
                    else if (UberRequest.SurgeMultiplier >= 2.0 && UberRequest.SurgeMultiplierHref != null)
                    {
                        // Surge pricing in effect. Redirect for confirmation.
                        uberTripOptionModel.SurgeConfirmationId = UberRequest.SurgeConfirmationId;

                        NavigationService.NavigateTo("/Views/UberSurgePricingConfirmation.xaml?href=" + HttpUtility.UrlEncode(UberRequest.SurgeMultiplierHref.ToString()));
                    }
                    else
                    {
                        switch (UberRequest.Status)
                        {
                            case "no_drivers_available":
                                ShowPopup(CustomPopupMessageType.Information, AppResources.UberNoDriversPopupText, AppResources.CustomPopupGenericOkMessage, null);

                                TripStatus = AppResources.UberTripStatusNoDrivers;
                                RetryOrCancelVisibility = Visibility.Visible;
                                break;
                            case "driver_canceled":
                                ShowPopup(CustomPopupMessageType.Information, AppResources.UberDriverCanceledPopupText, AppResources.CustomPopupGenericOkMessage, null);

                                TripStatus = AppResources.UberTripStatusDriverCanceled;
                                RetryOrCancelVisibility = Visibility.Visible;
                                break;
                            default:
                                TripStatus = AppResources.UberTripStatusProcessing;
                                ShowPopup(CustomPopupMessageType.Information, AppResources.UberRequestRecievedPopupText, AppResources.CustomPopupGenericOkMessage, null);

                                IEnumerable<Entities.UberTrip> uberTrips = UnitOfWork.UberTripRepository.GetAll();
                                if (!uberTrips.Any(x => x.RequestId == this.UberRequest.RequestId))
                                {
                                    UnitOfWork.UberTripRepository.Insert(new Entities.UberTrip(this.UberRequest.RequestId, LocationText, DestinationText, DateTime.Now));
                                    UnitOfWork.Save();
                                }

                                this.timer.Interval = new TimeSpan(0, 0, 1);
                                this.timer.Start();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    TripDetailsVisibility = Visibility.Collapsed;
                    RetryOrCancelVisibility = Visibility.Collapsed;
                    FinishCancelVisibility = Visibility.Collapsed;
                    CancelVisibility = Visibility.Collapsed;

                    if (ex.Message == AppResources.ApiErrorPopupMessageUnauthorizedError)
                    {
                        HideHeaderLoader();

                        TripStatus = AppResources.UberTripStatusUnauthorized;

                        RetryOrCancelVisibility = Visibility.Visible;

                        NavigationService.NavigateTo("/Views/UberAuthentication.xaml");
                    }
                    else if (ex.Message == AppResources.ApiErrorPopupMessageNotFoundError)
                    {
                        TripStatus = AppResources.UberTripStatusCompleted;

                        FinishCancelVisibility = Visibility.Visible;
                    }
                    else
                    {
                        TripStatus = AppResources.UberTripStatusError;

                        RetryOrCancelVisibility = Visibility.Visible;

                        ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);
                    }
                }

                HideHeaderLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(requestUber);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ShowHeaderLoader();

            this.timer.Stop();

            Action checkForUpdates = async () =>
            {
                if (user != null)
                {
                    try
                    {
                        this.UberRequest = await uberService.GetUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.UberRequest.RequestId);

                        if (this.UberRequest.Status == "processing")
                        {
                            TripStatus = AppResources.UberTripStatusProcessing;
                            TripDetailsVisibility = Visibility.Collapsed;
                            RetryOrCancelVisibility = Visibility.Collapsed;
                            FinishCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Visible;

                            this.timer.Interval = new TimeSpan(0, 0, 5);
                            this.timer.Start();
                        }
                        else if (this.UberRequest.Status == "accepted")
                        {
                            TripStatus = AppResources.UberTripStatusAccepted;
                            TripDetailsVisibility = Visibility.Visible;
                            FinishCancelVisibility = Visibility.Visible;
                            RetryOrCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                            EtaVisibility = Visibility.Visible;

                            this.timer.Interval = new TimeSpan(0, 1, 0);
                            this.timer.Start();
                        }
                        else if (this.UberRequest.Status == "arriving")
                        {
                            TripStatus = AppResources.UberTripStatusArriving;
                            TripDetailsVisibility = Visibility.Visible;
                            FinishCancelVisibility = Visibility.Visible;
                            RetryOrCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                            EtaVisibility = Visibility.Visible;

                            this.timer.Interval = new TimeSpan(0, 1, 0);
                            this.timer.Start();
                        }
                        else if (this.UberRequest.Status == "in_progress")
                        {
                            TripStatus = AppResources.UberTripStatusInProgress;
                            TripDetailsVisibility = Visibility.Visible;
                            FinishCancelVisibility = Visibility.Visible;
                            RetryOrCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                            EtaVisibility = Visibility.Collapsed;

                            this.timer.Interval = new TimeSpan(0, 1, 0);
                            this.timer.Start();
                        }
                        else if (this.UberRequest.Status == "no_drivers_available")
                        {
                            ShowPopup(CustomPopupMessageType.Information, AppResources.UberNoDriversPopupText, AppResources.CustomPopupGenericOkMessage, null);

                            RetryOrCancelVisibility = Visibility.Visible;
                            FinishCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                            TripStatus = AppResources.UberTripStatusNoDrivers;
                            TripDetailsVisibility = Visibility.Collapsed;
                        }
                        else if (this.UberRequest.Status == "driver_canceled")
                        {
                            ShowPopup(CustomPopupMessageType.Information, AppResources.UberDriverCanceledPopupText, AppResources.CustomPopupGenericOkMessage, null);

                            RetryOrCancelVisibility = Visibility.Visible;
                            FinishCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                            TripStatus = AppResources.UberTripStatusDriverCanceled;
                            TripDetailsVisibility = Visibility.Collapsed;
                        }
                        else if (this.UberRequest.Status == "rider_canceled")
                        {
                            RetryOrCancelVisibility = Visibility.Collapsed;
                            FinishCancelVisibility = Visibility.Visible;
                            TripStatus = AppResources.UberTripStatusRiderCanceled;
                            TripDetailsVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                        }
                        else if (this.UberRequest.Status == "completed")
                        {
                            TripStatus = AppResources.UberTripStatusCompleted;
                            TripDetailsVisibility = Visibility.Visible;
                            FinishCancelVisibility = Visibility.Visible;
                            RetryOrCancelVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;
                        }
                    }
                    catch (Exception ex)
                    {
                        TripDetailsVisibility = Visibility.Collapsed;
                        RetryOrCancelVisibility = Visibility.Collapsed;
                        FinishCancelVisibility = Visibility.Collapsed;
                        CancelVisibility = Visibility.Collapsed;

                        if (ex.Message == AppResources.ApiErrorPopupMessageUnauthorizedError)
                        {
                            HideHeaderLoader();

                            TripStatus = AppResources.UberTripStatusUnauthorized;

                            RetryOrCancelVisibility = Visibility.Visible;

                            NavigationService.NavigateTo("/Views/UberAuthentication.xaml");
                        }
                        else if (ex.Message == AppResources.ApiErrorPopupMessageNotFoundError)
                        {
                            TripStatus = AppResources.UberTripStatusCompleted;

                            FinishCancelVisibility = Visibility.Visible;
                        }
                        else
                        {
                            TripStatus = AppResources.UberTripStatusError;

                            RetryOrCancelVisibility = Visibility.Visible;

                            ShowPopup(CustomPopupMessageType.Error, ex.Message, AppResources.CustomPopupGenericOkMessage, null);
                        }
                    }
                }

                HideHeaderLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(checkForUpdates);
        }

        private void CallNumber()
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.PhoneNumber = this.UberRequest.UberDriver.PhoneNumber;
            phoneCallTask.DisplayName = this.UberRequest.UberDriver.Name;

            phoneCallTask.Show();
        }

        private void Retry()
        {
            RequestUber();
        }
        private void Cancel()
        {
            if (TripStatus == AppResources.UberTripStatusDriverCanceled || TripStatus == AppResources.UberTripStatusNoDrivers || TripStatus == AppResources.UberTripStatusCompleted)
            {
                ShowPopupWithAction(CustomPopupMessageType.Information, AppResources.UberTripFinishedPopupText, AppResources.CustomPopupGenericOkGotItMessage, AppResources.CustomPopupGenericCancelMessage, FinishUberTrip, null, null);
            }
            else if (TripStatus == AppResources.UberTripStatusProcessing)
            {
                ShowPopupWithAction(CustomPopupMessageType.Information, AppResources.UberTripCancelConfirmPopupText, AppResources.CustomPopupGenericYesMessage, AppResources.CustomPopupGenericNoMessage, CanelUberTrip, null, null);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void Finish()
        {
            ShowPopupWithAction(CustomPopupMessageType.Information, AppResources.UberTripFinishedPopupText, AppResources.CustomPopupGenericOkGotItMessage, AppResources.CustomPopupGenericCancelMessage, FinishUberTrip, null, null);
        }

        private void FinishUberTrip()
        {
            if (SimpleIoc.Default.IsRegistered<Entities.UberTrip>())
                SimpleIoc.Default.Unregister<Entities.UberTrip>();

            // Clear any previously saved uber trips.
            UnitOfWork.UberTripRepository.RemoveAll();
            UnitOfWork.Save();

            NavigationService.GoBack();
        }

        private void CanelUberTrip()
        {
            this.timer.Stop();

            ShowHeaderLoader();

            Action cancelTrip = async () =>
            {
                if (user != null)
                {
                    bool tripCancelled = await uberService.DeleteUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.uberRequest.RequestId);

                    if (tripCancelled)
                    {
                        ShowPopup(CustomPopupMessageType.Sucess, AppResources.UberTripCancelled, AppResources.CustomPopupGenericOkMessage, null);
                    }
                    else
                    {
                        ShowPopup(CustomPopupMessageType.Sucess, AppResources.UberTripCancelFailure, AppResources.CustomPopupGenericOkMessage, null);
                    }

                    this.timer.Interval = new TimeSpan(0, 0, 1);
                    this.timer.Start();
                }

                HideHeaderLoader();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(cancelTrip);
        }

        private void Map()
        {
            NavigationService.NavigateTo("/Views/UberMap.xaml");
        }

        private void FakeAcceptTrip()
        {
            uberService.PutUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.uberRequest.RequestId, "accepted");
        }
        private void FakeComplete()
        {
            uberService.PutUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.uberRequest.RequestId, "completed");
        }
        private void FakeDriverCance1()
        {
            uberService.PutUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.uberRequest.RequestId, "driver_canceled");
        }
        private void FakeInProgress()
        {
            uberService.PutUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.uberRequest.RequestId, "in_progress");
        }
        private void FakeArrival()
        {
            uberService.PutUberRequest(CancellationToken.None, user.UberInfo.AccessToken, this.uberRequest.RequestId, "arriving");
        }

        #endregion

        #region Properties

        private string locationText = String.Empty;
        public string LocationText
        {
            get { return locationText; }
            set
            {
                locationText = value;
                RaisePropertyChanged("LocationText");
            }
        }

        private string destinationText = String.Empty;
        public string DestinationText
        {
            get { return destinationText; }
            set
            {
                destinationText = value;
                RaisePropertyChanged("DestinationText");
            }
        }

        private string tripStatus = AppResources.UberTripStatusRequesting;
        public string TripStatus
        {
            get { return tripStatus; }
            set
            {
                tripStatus = value;
                RaisePropertyChanged("TripStatus");
            }
        }

        private Visibility cancelVisibility = Visibility.Collapsed;
        public Visibility CancelVisibility
        {
            get { return cancelVisibility; }
            set
            {
                cancelVisibility = value;
                RaisePropertyChanged("CancelVisibility");
            }
        }

        private Visibility tripDetailsVisibility = Visibility.Collapsed;
        public Visibility TripDetailsVisibility
        {
            get { return tripDetailsVisibility; }
            set
            {
                tripDetailsVisibility = value;
                RaisePropertyChanged("TripDetailsVisibility");
            }
        }

        private Visibility finishCancelVisibility = Visibility.Collapsed;
        public Visibility FinishCancelVisibility
        {
            get { return finishCancelVisibility; }
            set
            {
                finishCancelVisibility = value;
                RaisePropertyChanged("FinishCancelVisibility");
            }
        }

        private Visibility etaVisibility = Visibility.Visible;
        public Visibility EtaVisibility
        {
            get { return etaVisibility; }
            set
            {
                etaVisibility = value;
                RaisePropertyChanged("EtaVisibility");
            }
        }
        
        private UberRequest uberRequest;
        public UberRequest UberRequest
        {
            get { return uberRequest; }
            set
            {
                uberRequest = value;
                RaisePropertyChanged("UberRequest");
            }
        }

        private Visibility retryOrCancelVisibility = Visibility.Collapsed;
        public Visibility RetryOrCancelVisibility
        {
            get { return retryOrCancelVisibility; }
            set
            {
                retryOrCancelVisibility = value;
                RaisePropertyChanged("RetryOrCancelVisibility");
            }
        }
        
        #endregion

        #region Commands

        public RelayCommand MapCommand
        {
            get { return new RelayCommand(Map); }
        }

        public RelayCommand CallNumberCommand
        {
            get { return new RelayCommand(CallNumber); }
        }

        public RelayCommand RetryButtonCommand
        {
            get { return new RelayCommand(Retry); }
        }

        public RelayCommand CancelButtonCommand
        {
            get { return new RelayCommand(Cancel); }
        }

        public RelayCommand FinishButtonCommand
        {
            get { return new RelayCommand(Finish); }
        }

        public RelayCommand FakeAcceptTripCommand
        {
            get { return new RelayCommand(FakeAcceptTrip); }
        }

        public RelayCommand FakeArrivalCommand
        {
            get { return new RelayCommand(FakeArrival); }
        }

        public RelayCommand FakeInProgressCommand
        {
            get { return new RelayCommand(FakeInProgress); }
        }

        public RelayCommand FakeDriverCancelCommand
        {
            get { return new RelayCommand(FakeDriverCance1); }
        }
      
        public RelayCommand FakeCompleteCommand
        {
            get { return new RelayCommand(FakeComplete); }
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
