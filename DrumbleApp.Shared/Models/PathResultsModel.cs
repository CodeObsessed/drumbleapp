using DrumbleApp.Shared.Entities;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using System;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Messages.Enums;
using DrumbleApp.Shared.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using DrumbleApp.Shared.ValueObjects;
using System.Collections.ObjectModel;
using DrumbleApp.Shared.Infrastructure.Extensions;

namespace DrumbleApp.Shared.Models
{
    public sealed class PathResultsModel : ViewModelBase
    {
        private IBumbleApiService BumbleApi;
        private User user;
        private CancellationTokenSource cancellationTokenSource;

        public Address Location { get; private set; }
        public Address Destination { get; private set; }
        public IEnumerable<PathOption> PathOptions { get; private set; }

        public PathResultsModel(Address location, Address destination, IEnumerable<PathOption> pathOptions, IBumbleApiService BumbleApi, User user)
        {
            this.BumbleApi = BumbleApi;
            this.user = user;
            this.Location = location;
            this.Destination = destination;
            this.PathOptions = pathOptions;
            int numResults = pathOptions.Where(x => !x.IsUber).Count();
            if (numResults > 0)
            {
                TripAVisibility = Visibility.Visible;
            }
            if (numResults > 1)
            {
                TripBVisibility = Visibility.Visible;
            }
            if (numResults > 2)
            {
                TripCVisibility = Visibility.Visible;
            }
            if (numResults > 3)
            {
                TripDVisibility = Visibility.Visible;
            }
            if (numResults > 4)
            {
                TripEVisibility = Visibility.Visible;
            }
        }

        #region Public Functions

        public void SetSelectedPathOption(string letter)
        {
            if (!PathOptions.Any(x => x.Letter == letter) && PathOptions.Count() == 1)
            {
                letter = "A";
                PathOptions.First().Letter = "A";
            }

            SetChosenPathImage(letter);

            SelectedPathOption = PathOptions.Where(x => x.Letter == letter).First();
        }

        public void NextPathOption()
        {
            int numPaths = PathOptions.Where(x => !x.IsUber).Count();

            if (numPaths == 1)
                return;

            var pathOptionsList = PathOptions.ToList();

            for (int i = 0; i < numPaths; i++)
            {
                if (pathOptionsList[i].Letter == SelectedPathOption.Letter)
                {
                    if (i == numPaths)
                        SelectedPathOption = pathOptionsList[0];
                    else
                        SelectedPathOption = pathOptionsList[(i + 1) % (numPaths)];

                    break;
                }
            }

            SetChosenPathImage(SelectedPathOption.Letter);
        }

        public void PreviousPathOption()
        {
            int numPaths = PathOptions.Where(x => !x.IsUber).Count();

            if (numPaths == 1)
                return;

            var pathOptionsList = PathOptions.ToList();

            for (int i = 0; i < numPaths; i++)
            {
                if (pathOptionsList[i].Letter == SelectedPathOption.Letter)
                {
                    if (i == 0)
                        SelectedPathOption = pathOptionsList.Where(x => !x.IsUber).Last();
                    else
                        SelectedPathOption = pathOptionsList[(i - 1) % (numPaths)];

                    break;
                }
            }

            SetChosenPathImage(SelectedPathOption.Letter);
        }

        #endregion

        #region Private Functions

        private void SetSelectedStage()
        {
            if (SelectedPathOption.IsUber)
                return;

            if (DateTime.Now <= SelectedPathOption.Stages.First().StartTime)
            {
                SelectedPathOption.SelectedStage = SelectedPathOption.Stages.First();
                return;
            }

            if (DateTime.Now >= SelectedPathOption.Stages.Last().EndTime)
            {
                SelectedPathOption.SelectedStage = SelectedPathOption.Stages.Last();
                return;
            }

            foreach (Stage stage in SelectedPathOption.Stages)
            {
                if (stage.StartTime <= DateTime.Now && stage.EndTime > DateTime.Now)
                {
                    SelectedPathOption.SelectedStage = stage;
                    return;
                }
            }
        }

        private void ClearImages()
        {
            TripAImage = "/Images/64/GY/DrumbleSymbol-GreyA.png";
            TripBImage = "/Images/64/GY/DrumbleSymbol-GreyB.png";
            TripCImage = "/Images/64/GY/DrumbleSymbol-GreyC.png";
            TripDImage = "/Images/64/GY/DrumbleSymbol-GreyD.png";
            TripEImage = "/Images/64/GY/DrumbleSymbol-GreyE.png";
        }

        private void SetChosenPathImage(string letter)
        {
            ClearImages();

            switch (letter)
            {
                case "A":
                    TripAImage = "/Images/64/LB/DrumbleSymbol-BlueA.png";
                    break;
                case "B":
                    TripBImage = "/Images/64/LB/DrumbleSymbol-BlueB.png";
                    break;
                case "C":
                    TripCImage = "/Images/64/LB/DrumbleSymbol-BlueC.png";
                    break;
                case "D":
                    TripDImage = "/Images/64/LB/DrumbleSymbol-BlueD.png";
                    break;
                case "E":
                    TripEImage = "/Images/64/LB/DrumbleSymbol-BlueE.png";
                    break;
            }
        }

        private void TripA()
        {
            SetSelectedPathOption("A");
        }

        private void TripB()
        {
            SetSelectedPathOption("B");
        }

        private void TripC()
        {
            SetSelectedPathOption("C");
        }

        private void TripD()
        {
            SetSelectedPathOption("D");
        }

        private void TripE()
        {
            SetSelectedPathOption("E");
        }

        private void PopulateTrip(Guid id)
        {
            if (SelectedPathOption.Trip != null)
            {
                TripMessage.Send(SelectedPathOption.Trip);
                return;
            }

            Action getTrip = async () =>
            {
                LoadingBarMessage.Send(LoadingBarMessageReason.Show);

                try
                {
                    if (cancellationTokenSource == null)
                        cancellationTokenSource = new CancellationTokenSource();
                    else
                    {
                        cancellationTokenSource.Cancel();
                        cancellationTokenSource = new CancellationTokenSource();
                    }
                    Trip trip = await BumbleApi.Trip(cancellationTokenSource.Token, user, id);

                    cancellationTokenSource = null;

                    PathOptions.Where(x => x.TripId == id).FirstOrDefault().Trip = trip;

                    SimpleIoc.Default.Unregister<PathResultsModel>();

                    SimpleIoc.Default.Register<PathResultsModel>(() =>
                    {
                        return this;
                    });

                    TripMessage.Send(trip);
                }
                catch (Exception)
                {
                }

                cancellationTokenSource = null;

                LoadingBarMessage.Send(LoadingBarMessageReason.Hide);

            };

            DispatcherHelper.CheckBeginInvokeOnUI(getTrip);
        }

        #endregion

        #region Properties

        private ObservableCollection<RouteStop> selectedRouteStops = new ObservableCollection<RouteStop>();
        public ObservableCollection<RouteStop> SelectedRouteStops
        {
            get { return selectedRouteStops; }

        }

        private PathOption selectedPathOption;
        public PathOption SelectedPathOption
        {
            get { return selectedPathOption; }
            set
            {
                SelectedRouteStops.Clear();
                SelectedRouteStops.AddRange(value.RouteStops);

                selectedPathOption = value;
                RaisePropertyChanged("SelectedPathOption");

                PopulateTrip(value.TripId);
                SetSelectedStage();
            }
        }

        public string TotalAnnouncementCountDisplay
        {
            get
            {
                if (this.PathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Any(x => x.Announcements.Count() > 0))
                    return this.PathOptions.Where(x => !x.IsUber).SelectMany(x => x.Stages).Sum(x => x.Announcements.Count()).ToString();

                return String.Empty;
            }
        }

        #endregion

        #region Image

        private string tripAImage = "/Images/64/GY/DrumbleSymbol-GreyA.png";
        public string TripAImage
        {
            get { return tripAImage; }
            set
            {
                tripAImage = value;
                RaisePropertyChanged("TripAImage");
            }
        }

        private string tripBImage = "/Images/64/GY/DrumbleSymbol-GreyB.png";
        public string TripBImage
        {
            get { return tripBImage; }
            set
            {
                tripBImage = value;
                RaisePropertyChanged("TripBImage");
            }
        }

        private string tripCImage = "/Images/64/GY/DrumbleSymbol-GreyC.png";
        public string TripCImage
        {
            get { return tripCImage; }
            set
            {
                tripCImage = value;
                RaisePropertyChanged("TripCImage");
            }
        }

        private string tripDImage = "/Images/64/GY/DrumbleSymbol-GreyD.png";
        public string TripDImage
        {
            get { return tripDImage; }
            set
            {
                tripDImage = value;
                RaisePropertyChanged("TripDImage");
            }
        }

        private string tripEImage = "/Images/64/GY/DrumbleSymbol-GreyE.png";
        public string TripEImage
        {
            get { return tripEImage; }
            set
            {
                tripEImage = value;
                RaisePropertyChanged("TripEImage");
            }
        }

        #endregion

        #region Visibility

        private Visibility tripAVisibility = Visibility.Collapsed;
        public Visibility TripAVisibility
        {
            get { return tripAVisibility; }
            set
            {
                tripAVisibility = value;
                RaisePropertyChanged("TripAVisibility");
            }
        }

        private Visibility tripBVisibility = Visibility.Collapsed;
        public Visibility TripBVisibility
        {
            get { return tripBVisibility; }
            set
            {
                tripBVisibility = value;
                RaisePropertyChanged("TripBVisibility");
            }
        }

        private Visibility tripCVisibility = Visibility.Collapsed;
        public Visibility TripCVisibility
        {
            get { return tripCVisibility; }
            set
            {
                tripCVisibility = value;
                RaisePropertyChanged("TripCVisibility");
            }
        }

        private Visibility tripDVisibility = Visibility.Collapsed;
        public Visibility TripDVisibility
        {
            get { return tripDVisibility; }
            set
            {
                tripDVisibility = value;
                RaisePropertyChanged("TripDVisibility");
            }
        }

        private Visibility tripEVisibility = Visibility.Collapsed;
        public Visibility TripEVisibility
        {
            get { return tripEVisibility; }
            set
            {
                tripEVisibility = value;
                RaisePropertyChanged("TripEVisibility");
            }
        }

        #endregion

        #region Commands

        public RelayCommand TripACommand
        {
            get { return new RelayCommand(TripA); }
        }

        public RelayCommand TripBCommand
        {
            get { return new RelayCommand(TripB); }
        }

        public RelayCommand TripCCommand
        {
            get { return new RelayCommand(TripC); }
        }

        public RelayCommand TripDCommand
        {
            get { return new RelayCommand(TripD); }
        }

        public RelayCommand TripECommand
        {
            get { return new RelayCommand(TripE); }
        }

        #endregion
    }
}
