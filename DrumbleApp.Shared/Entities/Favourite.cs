using DrumbleApp.Shared.Converters;
using DrumbleApp.Shared.Messages.Classes;
using DrumbleApp.Shared.Resources;
using DrumbleApp.Shared.ValueObjects;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;

namespace DrumbleApp.Shared.Entities
{
    public sealed class Favourite
    {
        public Guid Id { get; private set; }
        public Coordinate Point { get; private set; }
        public string Text { get; private set; }
        public DateTime LastUsedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public int NumberOfUses { get; private set; }
        public string RelativeLastUsedDateString { get; private set; }

        public Favourite(Coordinate point, string text, DateTime lastUsedDate, DateTime createdDate, int numberOfUses)
        {
            this.Id = Guid.NewGuid();
            this.Point = point;
            this.Text = text;
            this.LastUsedDate = lastUsedDate;
            this.CreatedDate = createdDate;
            this.NumberOfUses = numberOfUses;
            this.RelativeLastUsedDateString = string.Format("{0} {1}", AppResources.FavouriteLastUsedText, TimeConverter.ToRelativeDateString(lastUsedDate.ToLocalTime(), true)); ;
        }

        public Favourite(Guid id, Coordinate point, string text, DateTime lastUsedDate, DateTime createdDate, int numberOfUses)
            : this(point, text, lastUsedDate, createdDate, numberOfUses)
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

        #region Local Functions

        private void RemoveFromFavourites()
        {
            FavouriteMessage.Send(this, Messages.Enums.FavouriteMessageReason.RemoveFromFavourites);
        }

        #endregion

        #region Commands

        public RelayCommand RemoveFromFavouritesCommand
        {
            get { return new RelayCommand(RemoveFromFavourites); }
        }

        #endregion
    }
}
