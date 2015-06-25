using DrumbleApp.Shared.Entities;
using DrumbleApp.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrumbleApp.Shared.Data.Factories
{
    public static class EntityModelFactory
    {
        public static TransportMode Create(Data.Models.TransportMode transportMode)
        {
            if (transportMode == null)
            {
                throw new ArgumentNullException("transportMode");
            }

            return new TransportMode(transportMode.Id, transportMode.ApplicationTransportMode, transportMode.IsEnabled);
        }

        public static OperatorSetting Create(Data.Models.OperatorSetting operatorSetting)
        {
            if (operatorSetting == null)
            {
                throw new ArgumentNullException("operatorSetting");
            }

            return new OperatorSetting(operatorSetting.Id, operatorSetting.OperatorName, operatorSetting.IsEnabled, operatorSetting.HasBeenModified);
        }

        public static AppSetting Create(DrumbleApp.Shared.Data.Models.AppSetting appSetting)
        {
            if (appSetting == null)
            {
                throw new ArgumentNullException("appSetting");
            }

            return new AppSetting(appSetting.Id, appSetting.ApplicationSetting, appSetting.SettingValue);
        }

        public static PlaceOfInterestCategory Create(DrumbleApp.Shared.Data.Models.PlaceOfInterestCategory placeOfInterestCategory)
        {
            if (placeOfInterestCategory == null)
                return null;

            return new PlaceOfInterestCategory(placeOfInterestCategory.Id, placeOfInterestCategory.Category, placeOfInterestCategory.ImageBinary, placeOfInterestCategory.IsChecked);
        }

        public static SearchItemCategory Create(DrumbleApp.Shared.Data.Models.SearchItemCategory searchItemCategory)
        {
            if (searchItemCategory == null)
            {
                throw new ArgumentNullException("searchItemCategory");
            }

            return new SearchItemCategory(searchItemCategory.Id, searchItemCategory.ImageBinary, searchItemCategory.Text);
        }

        public static User Create(DrumbleApp.Shared.Data.Models.User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return new User(user.Id, new ValueObjects.Token(user.Token), CreateCountry(user.CountryCode, user.CountryName), user.DismissedLocationPopup, new Coordinate(user.Latitude, user.Longitude), user.LastLocationUpdate, user.DismissedRateAppPopup, user.DismissedSignUpPopup, user.IsBumbleRegistered, (string.IsNullOrEmpty(user.Email)) ? null : new Email(user.Email), user.FirstName, user.LastName, user.IsFacebookRegistered, user.IsTwitterRegistered, (!user.IsFacebookRegistered) ? null : new FacebookInfo(user.FacebookAccessToken, user.FacebookId), (!user.IsTwitterRegistered) ? null : new TwitterInfo(user.TwitterAccessToken, user.TwitterAccessTokenSecret, user.TwitterId), user.TwitterHandle, user.AppUsageCount, user.DismissedLoginUberPopup, (!user.IsUberAuthenticated) ? null : new UberInfo(user.UberAccessToken, user.UberRefreshToken), user.IsUberAuthenticated);
        }

        private static Country CreateCountry(string countryCode, string countryName)
        {
            if (String.IsNullOrEmpty(countryCode))
                return null;

            return new Country(countryCode, countryName);
        }

        public static PublicTransportOperator Create(DrumbleApp.Shared.Data.Models.PublicTransportOperator publicTransportOperator)
        {
            if (publicTransportOperator == null)
            {
                throw new ArgumentNullException("publicTransportOperator");
            }
            PublicTransportOperator modelPublicTransportOperator = new PublicTransportOperator(publicTransportOperator.Id, publicTransportOperator.Name, publicTransportOperator.DisplayName, publicTransportOperator.Category, publicTransportOperator.RouteImageUrl, publicTransportOperator.TwitterHandle, publicTransportOperator.FacebookPage, publicTransportOperator.WebsiteAddress, publicTransportOperator.ContactEmail, publicTransportOperator.ContactNumber, publicTransportOperator.IsPublic);

            return modelPublicTransportOperator;
        }


        public static IEnumerable<PublicTransportOperator> Create(IEnumerable<DrumbleApp.Shared.Data.Models.PublicTransportOperator> publicTransportOperators)
        {
            if (publicTransportOperators == null)
            {
                throw new ArgumentNullException("publicTransportOperators");
            }

            return publicTransportOperators.Select(x => Create(x));
        }

        public static CacheSetting Create(DrumbleApp.Shared.Data.Models.CacheSetting cacheSetting)
        {
            if (cacheSetting == null)
                return null;

            return new CacheSetting(cacheSetting.Id, cacheSetting.LastRefreshedDateUtc, cacheSetting.ResourceType);
        }

        public static Weather Create(DrumbleApp.Shared.Data.Models.Weather weather)
        {
            if (weather == null)
                return null;

            return new Weather(weather.Id, weather.Icon, weather.LastRefreshedDate);
        }

        public static Recent Create(Models.Recent recentTrip)
        {
            if (recentTrip == null)
                return null;

            return new Recent(recentTrip.Id, new Coordinate(recentTrip.Latitude, recentTrip.Longitude), recentTrip.Text, recentTrip.LastUsedDate, recentTrip.CreatedDate, recentTrip.IsFavourite, recentTrip.NumberOfUses);
        }

        public static Favourite CreateFavourite(Models.Recent recentTrip)
        {
            if (recentTrip == null)
                return null;

            return new Favourite(recentTrip.Id, new Coordinate(recentTrip.Latitude, recentTrip.Longitude), recentTrip.Text, recentTrip.LastUsedDate, recentTrip.CreatedDate, recentTrip.NumberOfUses);
        }

        public static Path Create(Models.Path path, IEnumerable<PublicTransportOperator> publicTransportOperators)
        {
            if (path == null)
                return null;

            return new Path(path.TripId, path.StartDate, path.EndDate, path.LocationText, path.DestinationText, new Coordinate(path.StartLatitude, path.StartLongitude), new Coordinate(path.EndLatitude, path.EndLongitude), path.IsPinned, path.JsonObject, path.Order, publicTransportOperators);
        }

        public static UberTrip Create(Models.UberTrip uberTrip)
        {
            if (uberTrip == null)
                return null;

            return new UberTrip(uberTrip.Id, uberTrip.RequestId, uberTrip.Location, uberTrip.Destination, uberTrip.CreatedDate);
        }
    }
}
