using DrumbleApp.Shared.Enums;
using DrumbleApp.Shared.Infrastructure.Analytics;
using DrumbleApp.Shared.Interfaces;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DrumbleApp.Shared.ViewModels
{
    public sealed class DateTimeSelectionViewModel : AnalyticsBase, IDisposable
    {
        private bool isDeparting = true;
        private bool isInterval = true;

        public DateTimeSelectionViewModel(IAggregateService aggregateService)
            : base(ApplicationPage.DateTimeSelection, aggregateService)
        {
            Messenger.Default.Register<DepartureTimeSelectionMessage>(this, (action) => SetSelectionMode(action));
        }

        #region Local Functions

        protected override void PageLoaded()
        {
            base.PageLoaded();
        }

        protected override void PageUnloaded()
        {
            base.PageUnloaded();

            Messenger.Default.Unregister<FlickMessage>(this);
        }

        private void SpecifyDeparting()
        {
            SetHeader();

            DepartureVisibility = Visibility.Visible;
            ArrivalVisibility = Visibility.Collapsed;

            isDeparting = true;
        }

        private void SpecifyArriving()
        {
            SetHeader();
            DepartureVisibility = Visibility.Collapsed;
            ArrivalVisibility = Visibility.Visible;

            isDeparting = false;
        }

        private void SetSelectionMode(DepartureTimeSelectionMessage mode)
        {
            switch (mode.Reason)
            {
                case Messages.Enums.DepartureTimeSelectionMessageReason.Custom:
                    SpecifyDate();
                    break;
                case Messages.Enums.DepartureTimeSelectionMessageReason.Interval:
                    SpecifyInterval();
                    break;
            }
        }

        private void SpecifyDateSwipe()
        {
            SpecifyDate();
        }

        private void SpecifyIntervalSwipe()
        {
            SpecifyInterval();
        }

        private void SpecifyDateClick()
        {
            SpecifyDate();
        }

        private void SpecifyIntervalClick()
        {
            SpecifyInterval();
        }

        private void SetHeader()
        {
            if (this.isDeparting)
                PageTitleMessage.Send(AppResources.HeaderDateTimeDeparture);
            else
                PageTitleMessage.Send(AppResources.HeaderDateTimeArrival);
        }

        private void SpecifyDate()
        {
            SetHeader();

            this.isInterval = false;
            if (PivotControlSelectedIndex != 1)
            PivotControlSelectedIndex = 1;
            SpecifyDateVisibility = Visibility.Visible;
            SelectIntervalVisibility = Visibility.Collapsed;
            SelectIntervalIcon = "/Images/64/W/IconTimeInterval-Off.png";
            CustomDateIcon = "/Images/64/LB/IconFullDate.png";
        }

        private void SpecifyInterval()
        {
            SetHeader();

            this.isInterval = true;
            if (PivotControlSelectedIndex != 0)
                PivotControlSelectedIndex = 0;
            SpecifyDateVisibility = Visibility.Collapsed;
            SelectIntervalVisibility = Visibility.Visible;
            SelectIntervalIcon = "/Images/64/LB/IconTimeInterval.png";
            CustomDateIcon = "/Images/64/W/IconFullDate-Off.png";
        }

        private void Submit()
        {
            DepartureTimeMessage.Send(SelectedDateTime, isDeparting);

            NavigationService.GoBack();
        }

        #endregion

        #region Properties

        private PredefinedDepartureTime selectedTime;
        public PredefinedDepartureTime SelectedTime
        {
            get { return selectedTime; }
            set
            {
                selectedTime = value;
                RaisePropertyChanged("SelectedTime");

                if (value != null)
                {
                    DepartureTimeMessage.Send(value, isDeparting);

                    SelectedTime = null;
                    NavigationService.GoBack();
                }
            }
        }

        public ObservableCollection<PredefinedDepartureTime> Times
        {
            get
            {
                ObservableCollection<PredefinedDepartureTime> predefinedDepartureTimes = new ObservableCollection<PredefinedDepartureTime>();
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(0));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(15));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(30));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(45));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(60));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(90));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(120));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(240));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(480));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(960));
                predefinedDepartureTimes.Add(new PredefinedDepartureTime(1440));
                return predefinedDepartureTimes;
            }
        }

        private int pivotControlSelectedIndex = 0;
        public int PivotControlSelectedIndex
        {
            get { return pivotControlSelectedIndex; }
            set
            {
                pivotControlSelectedIndex = value;

                if (pivotControlSelectedIndex == 0)
                {
                    SpecifyIntervalSwipe();
                }
                else
                {
                    SpecifyDateSwipe();
                }
                RaisePropertyChanged("PivotControlSelectedIndex");
            }
        }

        private string customDateIcon = "/Images/64/W/IconFullDate-Off.png";
        public string CustomDateIcon
        {
            get { return customDateIcon; }
            set
            {
                customDateIcon = value;
                RaisePropertyChanged("CustomDateIcon");
            }
        }

        private string selectIntervalIcon = "/Images/64/W/IconTimeInterval-Off.png";
        public string SelectIntervalIcon
        {
            get { return selectIntervalIcon; }
            set
            {
                selectIntervalIcon = value;
                RaisePropertyChanged("SelectIntervalIcon");
            }
        }

        private DateTime selectedDateTime = DateTime.Now;
        public DateTime SelectedDateTime
        {
            get { return selectedDateTime; }
            set
            {
                selectedDateTime = value;
                RaisePropertyChanged("SelectedDateTime");
            }
        }

        private Visibility departureVisibility = Visibility.Visible;
        public Visibility DepartureVisibility
        {
            get { return departureVisibility; }
            set
            {
                departureVisibility = value;
                RaisePropertyChanged("DepartureVisibility");
            }
        }

        private Visibility arrivalVisibility = Visibility.Collapsed;
        public Visibility ArrivalVisibility
        {
            get { return arrivalVisibility; }
            set
            {
                arrivalVisibility = value;
                RaisePropertyChanged("ArrivalVisibility");
            }
        }

        private Visibility selectIntervalVisibility = Visibility.Visible;
        public Visibility SelectIntervalVisibility
        {
            get { return selectIntervalVisibility; }
            set
            {
                selectIntervalVisibility = value;
                RaisePropertyChanged("SelectIntervalVisibility");
            }
        }

        private Visibility specifyDateVisibility = Visibility.Collapsed;
        public Visibility SpecifyDateVisibility
        {
            get { return specifyDateVisibility; }
            set
            {
                specifyDateVisibility = value;
                RaisePropertyChanged("SpecifyDateVisibility");
            }
        }

        #endregion

        #region Commands

        public RelayCommand SpecifyDateCommand
        {
            get { return new RelayCommand(SpecifyDateClick); }
        }

        public RelayCommand SpecifyIntervalCommand
        {
            get { return new RelayCommand(SpecifyIntervalClick); }
        }

        public RelayCommand SubmitCommand
        {
            get { return new RelayCommand(Submit); }
        }

        public RelayCommand SpecifyDepartingCommand
        {
            get { return new RelayCommand(SpecifyDeparting); }
        }

        public RelayCommand SpecifyArrivingCommand
        {
            get { return new RelayCommand(SpecifyArriving); }
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
