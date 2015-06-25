using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows;
using System.Linq;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using DrumbleApp.Shared.Messages.Classes;
using Microsoft.Phone.Tasks;
using System.Globalization;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class TripSelectionViewModel : AnalyticsBase, IDisposable
    {
        private DispatcherTimer timer;
        private IUberService uberService;

        public TripSelectionViewModel(IAggregateService aggregateService, IUberService uberService)
            : base(ApplicationPage.TripSelection, aggregateService)
        {
            this.uberService = uberService;
            this.timer = new DispatcherTimer();
            this.timer.Tick += TimerTick;
        }

        #region Local Functions

        private void TimerTick(object sender, EventArgs e)
        {
            this.timer.Interval = new TimeSpan(0,1,0); 
            UpdateTripTimings();
        }

        private void UpdateTripTimings()
        {
            PathResultsModel = SimpleIoc.Default.GetInstance<PathResultsModel>();
            
            Action delayStageBarLoad = async () =>
            {
                await TaskEx.Delay(2500);

                double maxDurationInMinutes = 0;
                if (PathResultsModel.PathOptions.Any(x => !x.IsUber))
                {
                    DateTime end = PathResultsModel.PathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Max(x => x.EndTime);
                    DateTime start = PathResultsModel.PathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Min(x => x.StartTime);
                    DateTime scaledStartTime = (DateTime.Now.AddHours(4) < start) ? start : DateTime.Now;
                    bool isScaledStartTime = (DateTime.Now.AddHours(4) < start) ? true : false;
                    maxDurationInMinutes = (PathResultsModel.PathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Max(x => x.EndTime) - scaledStartTime).TotalMinutes;
                    
                    foreach (PathOption pathOption in PathResultsModel.PathOptions.Where(x => !x.IsUber))
                    {
                        pathOption.UpdateStageDisplay(scaledStartTime, isScaledStartTime, maxDurationInMinutes, end);
                        pathOption.UpdateTileTimes();
                            
                    }
                }

                this.timer.Interval = new TimeSpan(0, 0, 60 - DateTime.Now.Second);
                this.timer.Start();
            };

            DispatcherHelper.CheckBeginInvokeOnUI(delayStageBarLoad);
        }

        private void UpdateUberTrip(bool refresh)
        {
            if (PathResultsModel.PathOptions.Any(x => x.IsUber))
                PathResultsModel.PathOptions.FirstOrDefault(x => x.IsUber).UpdateUberTrip(refresh);
        }

        private void RequestUber()
        {
            SimpleIoc.Default.Unregister<UberTripOptionModel>();

            SimpleIoc.Default.Register<UberTripOptionModel>(() =>
            {
                return new UberTripOptionModel(PathResultsModel.Location, PathResultsModel.Destination, SelectedPathOption.UberOption);
            });

            NavigationService.NavigateTo("/Views/UberTripDetails.xaml");
        }

        #endregion

        #region Overides

        protected override void PageLoaded()
        {
            base.PageLoaded();

            PageTitleMessage.Send(AppResources.HeaderTripSelection);

            UpdateTripTimings();

            UpdateUberTrip(false);
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            this.timer.Stop();
        }

        #endregion

        #region Properties

        private PathResultsModel pathResultsModel;
        public PathResultsModel PathResultsModel
        {
            get { return pathResultsModel; }
            set
            {
                pathResultsModel = value;
                RaisePropertyChanged("PathResultsModel");
            }
        }

        private PathOption selectedPathOption;
        public PathOption SelectedPathOption
        {
            get { return selectedPathOption; }
            set
            {
                selectedPathOption = value;
                RaisePropertyChanged("SelectedPathOption");

                if (selectedPathOption != null)
                {
                    if (selectedPathOption.IsUber)
                    {
                        if (!user.IsUberAuthenticated)
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri("https://m.uber.com/sign-up?client_id=" + selectedPathOption.UberOption.ClientId + "&product_id=" + selectedPathOption.UberOption.ProductId + "&pickup_latitude=" + PathResultsModel.Location.Location.Latitude.ToString(CultureInfo.InvariantCulture) + "&pickup_longitude=" + PathResultsModel.Location.Location.Longitude.ToString(CultureInfo.InvariantCulture) + "&dropoff_latitude=" + PathResultsModel.Destination.Location.Latitude.ToString(CultureInfo.InvariantCulture) + "&dropoff_longitude=" + PathResultsModel.Destination.Location.Longitude.ToString(CultureInfo.InvariantCulture));
                            webBrowserTask.Show();

                            SelectedPathOption = null;
                            return;
                        }
                        else
                        {
                            base.ShowPopupWithAction(CustomPopupMessageType.Information, AppResources.TripSelectionRequestUberPopupText, AppResources.CustomPopupGenericYesMessage, AppResources.CustomPopupGenericNoMessage, RequestUber, null, null);

                            return;
                        }
                    }
                    PathResultsModel.SetSelectedPathOption(value.Letter);

                    SimpleIoc.Default.Unregister<PathResultsModel>();

                    SimpleIoc.Default.Register<PathResultsModel>(() =>
                    {
                        return PathResultsModel;
                    });

                    SelectedPathOption = null;

                    NavigationService.NavigateTo("/Views/TripDetails.xaml");
                }
            }
        }

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
