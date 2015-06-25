using GalaSoft.MvvmLight;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DrumbleApp.Shared.Data.Models
{
    [Table]
    public sealed class User : ViewModelBase
    {
        private Guid id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
        public Guid Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    RaisePropertyChanging("Id");
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        private Guid token;
        [Column]
        public Guid Token
        {
            get { return token; }
            set
            {
                if (token != value)
                {
                    RaisePropertyChanging("Token");
                    token = value;
                    RaisePropertyChanged("Token");
                }
            }
        }

        private string email;
        [Column]
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    RaisePropertyChanging("Email");
                    email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        private string firstName;
        [Column]
        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (firstName != value)
                {
                    RaisePropertyChanging("FirstName");
                    firstName = value;
                    RaisePropertyChanged("FirstName");
                }
            }
        }

        private string lastName;
        [Column]
        public string LastName
        {
            get { return lastName; }
            set
            {
                if (lastName != value)
                {
                    RaisePropertyChanging("LastName");
                    lastName = value;
                    RaisePropertyChanged("LastName");
                }
            }
        }

        private string countryCode;
        [Column]
        public string CountryCode
        {
            get { return countryCode; }
            set
            {
                if (countryCode != value)
                {
                    RaisePropertyChanging("CountryCode");
                    countryCode = value;
                    RaisePropertyChanged("CountryCode");
                }
            }
        }

        private string countryName;
        [Column]
        public string CountryName
        {
            get { return countryName; }
            set
            {
                if (countryName != value)
                {
                    RaisePropertyChanging("CountryName");
                    countryName = value;
                    RaisePropertyChanged("CountryName");
                }
            }
        }

        private bool isBumbleRegistered;
        [Column]
        public bool IsBumbleRegistered
        {
            get { return isBumbleRegistered; }
            set
            {
                if (isBumbleRegistered != value)
                {
                    RaisePropertyChanging("IsBumbleRegistered");
                    isBumbleRegistered = value;
                    RaisePropertyChanged("IsBumbleRegistered");
                }
            }
        }

        private bool isFacebookRegistered;
        [Column]
        public bool IsFacebookRegistered
        {
            get { return isFacebookRegistered; }
            set
            {
                if (isFacebookRegistered != value)
                {
                    RaisePropertyChanging("IsFacebookRegistered");
                    isFacebookRegistered = value;
                    RaisePropertyChanged("IsFacebookRegistered");
                }
            }
        }

        private bool isTwitterRegistered;
        [Column]
        public bool IsTwitterRegistered
        {
            get { return isTwitterRegistered; }
            set
            {
                if (isTwitterRegistered != value)
                {
                    RaisePropertyChanging("IsTwitterRegistered");
                    isTwitterRegistered = value;
                    RaisePropertyChanged("IsTwitterRegistered");
                }
            }
        }

        private bool isUberAuthenticated;
        [Column]
        public bool IsUberAuthenticated
        {
            get { return isUberAuthenticated; }
            set
            {
                if (isUberAuthenticated != value)
                {
                    RaisePropertyChanging("IsUberAuthenticated");
                    isUberAuthenticated = value;
                    RaisePropertyChanged("IsUberAuthenticated");
                }
            }
        }

        private bool dismissedSignUpPopup;
        [Column]
        public bool DismissedSignUpPopup
        {
            get { return dismissedSignUpPopup; }
            set
            {
                if (dismissedSignUpPopup != value)
                {
                    RaisePropertyChanging("DismissedSignUpPopup");
                    dismissedSignUpPopup = value;
                    RaisePropertyChanged("DismissedSignUpPopup");
                }
            }
        }

        private bool dismissedRateAppPopup;
        [Column]
        public bool DismissedRateAppPopup
        {
            get { return dismissedRateAppPopup; }
            set
            {
                if (dismissedRateAppPopup != value)
                {
                    RaisePropertyChanging("DismissedRateAppPopup");
                    dismissedRateAppPopup = value;
                    RaisePropertyChanged("DismissedRateAppPopup");
                }
            }
        }

        private bool dismissedLocationPopup;
        [Column]
        public bool DismissedLocationPopup
        {
            get { return dismissedLocationPopup; }
            set
            {
                if (dismissedLocationPopup != value)
                {
                    RaisePropertyChanging("DismissedLocationPopup");
                    dismissedLocationPopup = value;
                    RaisePropertyChanged("DismissedLocationPopup");
                }
            }
        }

        private bool dismissedLoginUberPopup;
        [Column]
        public bool DismissedLoginUberPopup
        {
            get { return dismissedLoginUberPopup; }
            set
            {
                if (dismissedLoginUberPopup != value)
                {
                    RaisePropertyChanging("DismissedLoginUberPopup");
                    dismissedLoginUberPopup = value;
                    RaisePropertyChanged("DismissedLoginUberPopup");
                }
            }
        }
        
        private double latitude;
        [Column(CanBeNull = true)]
        public double Latitude
        {
            get { return latitude; }
            set
            {
                if (latitude != value)
                {
                    RaisePropertyChanging("Latitude");
                    latitude = value;
                    RaisePropertyChanged("Latitude");
                }
            }
        }

        private double longitude;
        [Column(CanBeNull = true)]
        public double Longitude
        {
            get { return longitude; }
            set
            {
                if (longitude != value)
                {
                    RaisePropertyChanging("Longitude");
                    longitude = value;
                    RaisePropertyChanged("Longitude");
                }
            }
        }

        private DateTime? lastLocationUpdate;
        [Column(CanBeNull = true)]
        public DateTime? LastLocationUpdate
        {
            get { return lastLocationUpdate; }
            set
            {
                if (lastLocationUpdate != value)
                {
                    RaisePropertyChanging("LastLocationUpdate");
                    lastLocationUpdate = value;
                    RaisePropertyChanged("LastLocationUpdate");
                }
            }
        }

        private string uberAccessToken;
        [Column]
        public string UberAccessToken
        {
            get { return uberAccessToken; }
            set
            {
                if (uberAccessToken != value)
                {
                    RaisePropertyChanging("UberAccessToken");
                    uberAccessToken = value;
                    RaisePropertyChanged("UberAccessToken");
                }
            }
        }

        private string uberRefreshToken;
        [Column]
        public string UberRefreshToken
        {
            get { return uberRefreshToken; }
            set
            {
                if (uberRefreshToken != value)
                {
                    RaisePropertyChanging("UberRefreshToken");
                    uberRefreshToken = value;
                    RaisePropertyChanged("UberRefreshToken");
                }
            }
        }

        private string facebookAccessToken;
        [Column]
        public string FacebookAccessToken
        {
            get { return facebookAccessToken; }
            set
            {
                if (facebookAccessToken != value)
                {
                    RaisePropertyChanging("FacebookAccessToken");
                    facebookAccessToken = value;
                    RaisePropertyChanged("FacebookAccessToken");
                }
            }
        }

        private string facebookId;
        [Column]
        public string FacebookId
        {
            get { return facebookId; }
            set
            {
                if (facebookId != value)
                {
                    RaisePropertyChanging("FacebookId");
                    facebookId = value;
                    RaisePropertyChanged("FacebookId");
                }
            }
        }

        private string twitterAccessToken;
        [Column]
        public string TwitterAccessToken
        {
            get { return twitterAccessToken; }
            set
            {
                if (twitterAccessToken != value)
                {
                    RaisePropertyChanging("TwitterAccessToken");
                    twitterAccessToken = value;
                    RaisePropertyChanged("TwitterAccessToken");
                }
            }
        }

        private string twitterAccessTokenSecret;
        [Column]
        public string TwitterAccessTokenSecret
        {
            get { return twitterAccessTokenSecret; }
            set
            {
                if (twitterAccessTokenSecret != value)
                {
                    RaisePropertyChanging("TwitterAccessTokenSecret");
                    twitterAccessTokenSecret = value;
                    RaisePropertyChanged("TwitterAccessTokenSecret");
                }
            }
        }

        private string twitterId;
        [Column]
        public string TwitterId
        {
            get { return twitterId; }
            set
            {
                if (twitterId != value)
                {
                    RaisePropertyChanging("TwitterId");
                    twitterId = value;
                    RaisePropertyChanged("TwitterId");
                }
            }
        }

        private string twitterHandle;
        [Column]
        public string TwitterHandle
        {
            get { return twitterHandle; }
            set
            {
                if (twitterHandle != value)
                {
                    RaisePropertyChanging("TwitterHandle");
                    twitterHandle = value;
                    RaisePropertyChanged("TwitterHandle");
                }
            }
        }

        private int appUsageCount;
        [Column]
        public int AppUsageCount
        {
            get { return appUsageCount; }
            set
            {
                if (appUsageCount != value)
                {
                    RaisePropertyChanging("AppUsageCount");
                    appUsageCount = value;
                    RaisePropertyChanged("AppUsageCount");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary version;

        public User() { }

        public User(Guid id, Guid token, string countryCode, string countryName, bool dismissedLocationPopup, double latitude, double longitude, DateTime? lastLocationUpdate, bool dismissedRateAppPopup, bool dismissedSignUpPopup, bool isBumbleRegistered, string email, bool isFacebookRegistered, bool isTwitterRegistered, string firstName, string lastName, string facebookId, string facebookAccessToken, string twitterAccessToken, string twitterAccessTokenSecret, string twitterId, string twitterHandle, int appUsageCount, bool dismissedLoginUberPopup, string uberAccessToken, string uberRefreshToken, bool isUberAuthenticated) 
        {
            this.Id = id;
            this.Token = token;
            this.CountryCode = countryCode;
            this.CountryName = countryName;
            this.DismissedLocationPopup = dismissedLocationPopup;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.LastLocationUpdate = lastLocationUpdate;
            this.DismissedRateAppPopup = dismissedRateAppPopup;
            this.DismissedSignUpPopup = dismissedSignUpPopup;
            this.dismissedLoginUberPopup = dismissedLoginUberPopup;
            this.IsBumbleRegistered = isBumbleRegistered;
            this.Email = email;
            this.IsFacebookRegistered = isFacebookRegistered;
            this.IsTwitterRegistered = isTwitterRegistered;
            this.IsUberAuthenticated = isUberAuthenticated;
            this.UberAccessToken = uberAccessToken;
            this.UberRefreshToken = uberRefreshToken;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.FacebookId = facebookId;
            this.FacebookAccessToken = facebookAccessToken;
            this.TwitterAccessToken = twitterAccessToken;
            this.TwitterAccessTokenSecret = twitterAccessTokenSecret;
            this.TwitterId = twitterId;
            this.TwitterHandle = twitterHandle;
            this.AppUsageCount = appUsageCount;
        }
    }
}
