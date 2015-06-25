using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using GalaSoft.MvvmLight;
using System.Windows;

namespace DrumbleApp.Shared.ValueObjects
{
    public sealed class WhereToPoint : ViewModelBase
    {
        private bool isPointA;

        public string Name
        {
            get 
            {
                if (Station != null)
                    return Station.Name;
                else if (SearchItem != null)
                    return SearchItem.Name;
                else if (CustomPoint != null)
                    return CustomPoint.Name;
                else
                    return string.Empty;
            }
        }

        private PublicStop station;
        public PublicStop Station
        {
            get { return station; }
            private set
            {
                station = value;
                RaisePropertyChanged("Station");
            }
        }

        private Entities.PlaceOfInterest searchItem;
        public Entities.PlaceOfInterest SearchItem
        {
            get { return searchItem; }
            private set
            {
                searchItem = value;
                RaisePropertyChanged("SearchItem");
            }
        }

        private Entities.PlaceOfInterest customPoint;
        public Entities.PlaceOfInterest CustomPoint
        {
            get { return customPoint; }
            private set
            {
                customPoint = value;
                RaisePropertyChanged("CustomPoint");
            }
        }

        public WhereToPoint(bool isPointA)
        {
            this.isPointA = isPointA;
        }

        public void SetAsStation(PublicStop station)
        {
            this.Station = station;
            this.SearchItem = null;
            this.CustomPoint = null;

            RaisePropertyChanged("Location");
        }

        public void SetAsPlaceOfInterest(Entities.PlaceOfInterest searchItem)
        {
            this.Station = null;
            this.CustomPoint = null;
            this.SearchItem = searchItem;

            RaisePropertyChanged("Location");
        }

        public void SetAsCustomPoint(Entities.PlaceOfInterest customPoint)
        {
            this.Station = null;
            this.SearchItem = null;
            this.CustomPoint = customPoint;

            RaisePropertyChanged("Location");
        }

        public void ClearValues()
        {
            this.Station = null;
            this.SearchItem = null;
            this.CustomPoint = null;

            this.Hide();
        }

        public bool HasValue()
        {
            return this.Station != null || this.SearchItem != null || this.CustomPoint != null;
        }

        public void Show()
        {
            PointVisibility = Visibility.Visible;
        }

        public void Hide()
        {
            PointVisibility = Visibility.Collapsed;
        }

        public void Select()
        {
            if (isPointA)
            {
                PageTitleMessage.Send(AppResources.HeaderPointA);
                PointImage = "/Images/64/LB/DrumbleSymbol-BlueA.png";
                PointChosenImage = "/Images/64/LB/MarkerBlueA.png";
            }
            else
            {
                PageTitleMessage.Send(AppResources.HeaderPointB);
                PointImage = "/Images/64/LB/DrumbleSymbol-BlueB.png";
                PointChosenImage = "/Images/64/LB/MarkerBlueB.png";
            }
        }

        public void Deselect()
        {
            if (isPointA)
            {
                PointImage = "/Images/64/GY/DrumbleSymbol-GreyA.png";
                PointChosenImage = "/Images/64/GY/MarkerGreyA.png";
            }
            else
            {
                PointImage = "/Images/64/GY/DrumbleSymbol-GreyB.png";
                PointChosenImage = "/Images/64/GY/MarkerGreyB.png";
            }
        }

        public Coordinate Location
        {
            get
            {
                if (!this.HasValue())
                    return null;

                if (this.Station == null && this.CustomPoint == null)
                    return this.SearchItem.Location;
                else if (this.Station == null && this.CustomPoint != null)
                    return this.CustomPoint.Location;
                else
                    return this.Station.Location;
            }
        }

        private string pointImage; 
        public string PointImage
        {
            get { return pointImage; }
            private set
            {
                pointImage = value;
                RaisePropertyChanged("PointImage");
            }
        }

        private string pointChosenImage;
        public string PointChosenImage
        {
            get { return pointChosenImage; }
            private set
            {
                pointChosenImage = value;
                RaisePropertyChanged("PointChosenImage");
            }
        }

        private Visibility pointVisibility = Visibility.Collapsed;
        public Visibility PointVisibility
        {
            get { return pointVisibility; }
            private set
            {
                pointVisibility = value;
                RaisePropertyChanged("PointVisibility");
            }
        }

        
    }
}
