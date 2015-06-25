using DrumbleApp.Domain.Models.ValueObjects;
using System;

namespace DrumbleApp.Domain.Models.Entities
{
    public sealed class User
    {
        public Guid Id { get; private set; }
        public Token Token { get; set; }
        public Country Country { get; set; }
        public Email Email { get; set; }
        public FacebookInfo FacebookInfo { get; set; }
        public TwitterInfo TwitterInfo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TwitterHandle { get; set; }
        public bool DismissedLocationPopup { get; set; }
        public bool DismissedRateAppPopup { get; set; }
        public bool DismissedSignUpPopup { get; set; }
        public bool IsTwitterRegistered { get; set; }
        public bool IsFacebookRegistered { get; set; }
        public bool IsBumbleRegistered { get; set; }
        public int AppUsageCount { get; set; }
        public Coordinate LastKnownGeneralLocation { get; private set; }
        public DateTime? LastLocationUpdate { get; private set; }
        public bool IsLocationUptodate 
        {
            get
            {
                if (LastLocationUpdate == null || LastLocationUpdate < DateTime.UtcNow.AddMinutes(-5))
                    return false;
                else
                    return true;
            }
        }

        public User()
            : this(new Token(Guid.Empty))
        {
        }

        public User(Token token)
        {
            this.Token = token;
            this.Id = Guid.NewGuid();
            this.DismissedLocationPopup = false;
            this.DismissedRateAppPopup = false;
            this.DismissedSignUpPopup = false;
            this.IsBumbleRegistered = false;
            this.Email = null;
            this.AppUsageCount = 0;
        }

        public User(Guid id, Token token)
            : this (token)
        {
            this.Id = id;
        }

        public User(Guid id, Token token, Country country, bool dismissedLocationPopup, Coordinate location, DateTime? lastLocationUpdate, bool dismissedRateAppPopup, bool dismissedSignUpPopup, bool isBumbleRegistered, Email email, string firstName, string lastName, bool isFacebookRegistered, bool isTwitterRegistered, FacebookInfo facebookInfo, TwitterInfo twitterInfo, string twitterHandle, int appUsageCount)
            : this(id, token)
        {
            this.Country = country;
            this.DismissedLocationPopup = dismissedLocationPopup;
            this.LastKnownGeneralLocation = location;
            this.LastLocationUpdate = lastLocationUpdate;
            this.DismissedRateAppPopup = dismissedRateAppPopup;
            this.DismissedSignUpPopup = dismissedSignUpPopup;
            this.IsBumbleRegistered = isBumbleRegistered;
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IsFacebookRegistered = isFacebookRegistered;
            this.IsTwitterRegistered = isTwitterRegistered;
            this.FacebookInfo = facebookInfo;
            this.TwitterInfo = twitterInfo;
            this.TwitterHandle = twitterHandle;
            this.AppUsageCount = appUsageCount;
        }

        public void UpdateLocation(Coordinate location)
        {
            this.LastKnownGeneralLocation = location;
            this.LastLocationUpdate = DateTime.UtcNow;
        }
    }
}
