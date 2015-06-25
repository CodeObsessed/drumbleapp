using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;

namespace DrumbleApp.Shared.Entities
{
    public sealed class Recent : ViewModelBase
    {
        public Guid Id { get; private set; }
        public Coordinate Point { get; private set; }
        public string Text { get; private set; }
        public DateTime LastUsedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public bool IsFavourite { get; private set; }
        public int NumberOfUses { get; private set; }
        public string RelativeLastUsedDateString { get; private set; }

        public Recent(Coordinate point, string text, DateTime lastUsedDate, DateTime createdDate)
            : this(point, text, lastUsedDate, createdDate, false, 1)
        {
        }

        public Recent(Coordinate point, string text, DateTime lastUsedDate, DateTime createdDate, bool isFavourite, int numberOfUses)
        {
            this.Id = Guid.NewGuid();
            this.Point = point;
            this.Text = text;
            this.LastUsedDate = lastUsedDate;
            this.CreatedDate = createdDate;
            this.IsFavourite = isFavourite;
            this.NumberOfUses = numberOfUses;
            this.RelativeLastUsedDateString = string.Format("{0} {1}", AppResources.RecentLastUsedText, TimeConverter.ToRelativeDateString(lastUsedDate.ToLocalTime(), true));
        }

        public Recent(Guid id, Coordinate point, string text, DateTime lastUsedDate, DateTime createdDate, bool isFavourite, int numberOfUses)
            : this(point, text, lastUsedDate, createdDate, isFavourite, numberOfUses)
        {
            this.Id = id;
        }

        public Visibility LowNumberOfUsesVisibility
        {
            get
            {
                if (this.NumberOfUses <= 99)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility MediumNumberOfUsesVisibility
        {
            get
            {
                if (this.NumberOfUses > 99 && this.NumberOfUses <= 999)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility HighNumberOfUsesVisibility
        {
            get
            {
                if (this.NumberOfUses > 999)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility AddToFavouritesVisibility
        {
            get
            {
                return (IsFavourite) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility RemoveFromFavouritesVisibility
        {
            get
            {
                return (IsFavourite) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #region Local Functions

        private void RemoveFromFavourites()
        {
            this.IsFavourite = false;

            RaisePropertyChanged("RemoveFromFavouritesVisibility");
            RaisePropertyChanged("AddToFavouritesVisibility");

            RecentTripMessage.Send(this, Messages.Enums.RecentTripMessageReason.RemoveFromFavourites);
        }

        private void AddToFavourites()
        {
            this.IsFavourite = true;

            RaisePropertyChanged("RemoveFromFavouritesVisibility");
            RaisePropertyChanged("AddToFavouritesVisibility");

            RecentTripMessage.Send(this, Messages.Enums.RecentTripMessageReason.AddToFavourites);
        }

        #endregion

        #region Commands

        public RelayCommand AddToFavouritesCommand
        {
            get { return new RelayCommand(AddToFavourites); }
        }

        public RelayCommand RemoveFromFavouritesCommand
        {
            get { return new RelayCommand(RemoveFromFavourites); }
        }

        #endregion
    }
}
